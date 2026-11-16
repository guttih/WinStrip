#include "SerialTransport.h"
#include <QBluetoothAddress>
#include <QBluetoothUuid>

SerialTransport::SerialTransport(QObject *parent) : QObject(parent)
{
    connectTimeout.setSingleShot(true);
    connectTimeout.setInterval(30000);
    connect(&connectTimeout, &QTimer::timeout, this, [this] {
        fail(tr("Bluetooth connection timed out. Check pairing and try again."));
    });
    connect(&session, &ProtocolSession::messageReceived, this, &SerialTransport::dataHasCome);
    connect(&session, &ProtocolSession::queryReply, this, &SerialTransport::queryReply);
    connect(&session, &ProtocolSession::errorOccurred, this, &SerialTransport::errorOccurred);
    connect(&session, &ProtocolSession::sessionFailed, this, &SerialTransport::fail);
}

SerialTransport::~SerialTransport()
{
    disconnectDevice();
}

void SerialTransport::setState(State value)
{
    if (connectionState != value) {
        connectionState = value;
        emit stateChanged(value);
    }
}

void SerialTransport::disconnectDevice()
{
    connectTimeout.stop();
    session.reset();
    // Remove callbacks before closing: a delayed signal from a previous socket
    // must never change the state of a newly connected device.
    if (serial) {
        auto old = serial;
        serial = nullptr;
        disconnect(old, nullptr, this, nullptr);
        old->close();
        old->deleteLater();
    }
    if (bluetooth) {
        auto old = bluetooth;
        bluetooth = nullptr;
        disconnect(old, nullptr, this, nullptr);
        old->abort();
        old->deleteLater();
    }
    setState(State::Disconnected);
}

void SerialTransport::fail(const QString &message)
{
    disconnectDevice();
    emit errorOccurred(message);
}

bool SerialTransport::connectToPort(const QString &name, int baudRate)
{
    disconnectDevice();
    setState(State::Connecting);
    auto port = new QSerialPort(this);
    serial = port;
    port->setPortName(name);
    port->setBaudRate(baudRate);
    port->setDataBits(QSerialPort::Data8);
    port->setParity(QSerialPort::NoParity);
    port->setStopBits(QSerialPort::OneStop);
    port->setFlowControl(QSerialPort::NoFlowControl);
    if (!port->open(QIODevice::ReadWrite)) {
        fail(tr("Cannot open %1: %2").arg(name, port->errorString()));
        return false;
    }
    connect(port, &QSerialPort::errorOccurred, this, [this, port](QSerialPort::SerialPortError error) {
        if (serial == port && error != QSerialPort::NoError)
            fail(tr("USB connection error: %1").arg(port->errorString()));
    });
    session.attach(port);
    // Opening USB can reset an ESP32. Let it boot, retaining incoming bytes.
    QTimer::singleShot(1500, port, [this, port] {
        if (serial == port) {
            setState(State::Connected);
            session.sendCommand(ALLSTATUS);
        }
    });
    return true;
}

void SerialTransport::connectToBluetooth(const QString &address)
{
    disconnectDevice();
    const QBluetoothAddress target(address);
    if (target.isNull()) {
        emit errorOccurred(tr("Select a Bluetooth device with a valid address."));
        return;
    }
    setState(State::Connecting);
    auto socket = new QBluetoothSocket(QBluetoothServiceInfo::RfcommProtocol, this);
    bluetooth = socket;
    connect(socket, &QBluetoothSocket::connected, this, [this, socket] {
        if (bluetooth != socket)
            return;
        connectTimeout.stop();
        session.attach(socket);
        setState(State::Connected);
        session.sendCommand(ALLSTATUS);
    });
    connect(socket, &QBluetoothSocket::disconnected, this, [this, socket] {
        if (bluetooth == socket)
            fail(tr("Bluetooth device disconnected. You can reconnect."));
    });
    connect(socket, &QBluetoothSocket::errorOccurred, this, [this, socket](QBluetoothSocket::SocketError) {
        if (bluetooth == socket)
            fail(tr("Bluetooth connection error: %1. Check pairing in Linux Bluetooth settings.")
                 .arg(socket->errorString()));
    });
    connectTimeout.start();
    socket->connectToService(target, QBluetoothUuid(QBluetoothUuid::ServiceClassUuid::SerialPort));
}
