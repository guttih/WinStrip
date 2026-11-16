#include "ProtocolSession.h"
#include <QJsonDocument>
#include <QJsonObject>

ProtocolSession::ProtocolSession(QObject *parent) : QObject(parent)
{
    timeout.setSingleShot(true);
    timeout.setInterval(5000);
    connect(&timeout, &QTimer::timeout, this, [this] {
        fail(pending == INVALID ? tr("Write timed out. Reconnect to try again.")
             : tr("%1 timed out. Reconnect to try again.").arg(SERIAL_COMMAND_STRING[pending]));
    });
}

void ProtocolSession::reset()
{
    ++generation;
    if (device)
        disconnect(device, nullptr, this, nullptr);
    device = nullptr;
    timeout.stop();
    queue.clear();
    pending = INVALID;
    framer.reset();
}

void ProtocolSession::attach(QIODevice *newDevice)
{
    reset();
    device = newDevice;
    if (!device)
        return;
    connect(device, &QIODevice::readyRead, this, &ProtocolSession::receive);
    connect(device, &QIODevice::bytesWritten, this, [this] {
        if (device && device->bytesToWrite() == 0 && pending == INVALID) {
            timeout.stop();
            pump();
        }
    });
    connect(device, &QIODevice::aboutToClose, this, [this] { fail(tr("Connection closed.")); });
    connect(device, &QObject::destroyed, this, [this] { fail(tr("Connection lost.")); });
}

void ProtocolSession::fail(const QString &message)
{
    reset();
    emit sessionFailed(message);
}

bool ProtocolSession::sendCommand(SERIAL_COMMAND command)
{
    if (command <= INVALID || command >= COMMAND_COUNT) {
        emit errorOccurred(tr("Unknown query."));
        return false;
    }
    return send(QString::fromLatin1(SERIAL_COMMAND_STRING[command]));
}

bool ProtocolSession::send(const QString &text)
{
    if (!device || !device->isWritable()) {
        emit errorOccurred(tr("Connect to a device first."));
        return false;
    }
    QByteArray payload = text.trimmed().toUtf8();
    if (payload.isEmpty() || payload.size() > MessageFramer::MaxPayload ||
        payload.contains('\n') || payload.contains('\r') || payload.contains('\0') || payload.endsWith('@')) {
        emit errorOccurred(tr("Send one complete command (maximum 3300 bytes)."));
        return false;
    }
    SERIAL_COMMAND command = INVALID;
    for (int i = STATUS; i < COMMAND_COUNT; ++i)
        if (payload == SERIAL_COMMAND_STRING[i])
            command = static_cast<SERIAL_COMMAND>(i);
    if (command == INVALID && !QJsonDocument::fromJson(payload).isObject()) {
        emit errorOccurred(tr("Use a known query or a JSON update object."));
        return false;
    }
    if (queue.size() >= 32) {
        emit errorOccurred(tr("Command queue is full. Wait for pending commands."));
        return false;
    }
    queue.enqueue({payload, command});
    const auto before = generation;
    pump();
    return generation == before;
}

void ProtocolSession::pump()
{
    if (reading || !device || pending != INVALID || device->bytesToWrite() > 0 || queue.isEmpty())
        return;
    while (device && pending == INVALID && !queue.isEmpty()) {
        const auto request = queue.dequeue();
        pending = request.command;
        const QByteArray wire = request.payload + '\n';
        const auto before = generation;
        const qint64 written = device->write(wire);
        if (before != generation)
            return;
        if (written != wire.size()) {
            fail(tr("Failed to write a complete command. Reconnect to try again."));
            return;
        }
        if (pending != INVALID || device->bytesToWrite() > 0) {
            timeout.start();
            return;
        }
        // JSON updates have no acknowledgement and never occupy a query slot.
    }
}

bool ProtocolSession::matchesReply(SERIAL_COMMAND command, const QByteArray &reply)
{
    switch (command) {
    case STATUS: return reply == "OK";
    case SEPARATOR: return reply == "@";
    case BUFFERSIZE: case PROGRAMCOUNT: case PIXELCOUNT: {
        bool ok = false;
        const int value = reply.toInt(&ok);
        return ok && value > 0;
    }
    default: break;
    }
    const auto doc = QJsonDocument::fromJson(reply);
    if (command == PROGRAMINFO)
        return doc.isArray();
    if (!doc.isObject())
        return false;
    const auto obj = doc.object();
    switch (command) {
    case ALLSTATUS: return obj.contains("programs") && obj.contains("com") && obj.contains("colors");
    case COLORS: return obj.contains("colors");
    case VALUES: return obj.contains("com") && obj.contains("values");
    case VALUES_COLORS: return obj.contains("com") && obj.contains("colors");
    case HARDWARE: return obj.contains("pixelcount");
    default: return false;
    }
}

void ProtocolSession::receive()
{
    if (!device)
        return;
    // Finish the entire read before sending another query: extra messages in
    // this burst cannot be misattributed to a query that has not been sent yet.
    reading = true;
    const auto before = generation;
    const auto bytes = device->readAll();
    framer.feed(bytes, [this] { return pending == SEPARATOR; },
        [this, before](const QByteArray &message) {
            if (generation != before)
                return;
            const QString text = QString::fromUtf8(message);
            const auto command = pending;
            if (command != INVALID && matchesReply(command, message)) {
                timeout.stop();
                pending = INVALID;
                if (device && device->bytesToWrite() > 0)
                    timeout.start();
                emit queryReply(command, text);
            }
            emit messageReceived(text);
        }, [this, before] {
            if (generation == before)
                fail(tr("Invalid or oversized reply. Reconnect to try again."));
        });
    reading = false;
    if (generation != before)
        framer.reset();
    pump();
}
