#ifndef PROTOCOLSESSION_H
#define PROTOCOLSESSION_H

#include <QObject>
#include <QPointer>
#include <QIODevice>
#include <QQueue>
#include <QTimer>
#include "MessageFramer.h"
#include "serialcommands.h"

// Owns protocol state, never the device. One query on the wire at a time.
class ProtocolSession : public QObject
{
    Q_OBJECT
public:
    explicit ProtocolSession(QObject *parent = nullptr);
    void attach(QIODevice *device);
    void reset();
    bool send(const QString &text);
    bool sendCommand(SERIAL_COMMAND command);
    bool hasPendingQuery() const { return pending != INVALID; }
    void setTimeoutInterval(int milliseconds) { timeout.setInterval(milliseconds); }
    static bool matchesReply(SERIAL_COMMAND command, const QByteArray &reply);
signals:
    void messageReceived(QString message);
    void queryReply(SERIAL_COMMAND command, QString reply);
    void errorOccurred(QString message);
    void sessionFailed(QString message);
private:
    struct Request { QByteArray payload; SERIAL_COMMAND command; };
    void pump();
    void receive();
    void fail(const QString &message);
    QPointer<QIODevice> device;
    QQueue<Request> queue;
    SERIAL_COMMAND pending = INVALID;
    MessageFramer framer;
    QTimer timeout;
    bool reading = false;
    quint64 generation = 0;
};
#endif
