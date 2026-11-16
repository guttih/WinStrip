#ifndef SERIALTRANSPORT_H
#define SERIALTRANSPORT_H

#include <QSerialPort>
#include <QBluetoothSocket>
#include <QTimer>
#include "ProtocolSession.h"

// Retains the existing application-facing name; both transports share a session.
class SerialTransport : public QObject
{
    Q_OBJECT
public:
    enum class State { Disconnected, Connecting, Connected };
    Q_ENUM(State)
    explicit SerialTransport(QObject *parent = nullptr);
    ~SerialTransport() override;
    bool connectToPort(const QString &name, int baudRate = 115200);
    void connectToBluetooth(const QString &address);
    void disconnectDevice();
    State state() const { return connectionState; }
    bool isConnected() const { return connectionState == State::Connected; }
    bool send(const char *text) { return session.send(QString::fromUtf8(text)); }
    bool sendCommand(SERIAL_COMMAND command) { return session.sendCommand(command); }
signals:
    void dataHasCome(QString message);
    void queryReply(SERIAL_COMMAND command, QString reply);
    void stateChanged(SerialTransport::State state);
    void errorOccurred(QString message);
private:
    void setState(State state);
    void fail(const QString &message);
    QSerialPort *serial = nullptr;
    QBluetoothSocket *bluetooth = nullptr;
    ProtocolSession session;
    QTimer connectTimeout;
    State connectionState = State::Disconnected;
};
#endif
