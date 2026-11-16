#include <QtTest>
#include <QComboBox>
#include <QPushButton>
#include <QSpinBox>
#include <QTabWidget>
#include <QLabel>
#include <QTextEdit>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QSocketNotifier>
#include <pty.h>
#include <unistd.h>
#include "MessageFramer.h"
#include "ProtocolSession.h"
#include "SerialStatus.h"
#include "mainwindow.h"

// A sequential device with controllable input, failed/short writes and delayed
// output progress. This exercises the same QIODevice code as USB and RFCOMM.
class FakeDevice : public QIODevice
{
public:
    FakeDevice() { open(ReadWrite); }
    QByteArray input, output;
    qint64 writeLimit = -1;
    bool failWrites = false;
    bool buffered = false;
    qint64 outstanding = 0;
    bool isSequential() const override { return true; }
    qint64 bytesAvailable() const override { return input.size() + QIODevice::bytesAvailable(); }
    qint64 bytesToWrite() const override { return outstanding; }
    void receive(const QByteArray &bytes) { input += bytes; emit readyRead(); }
    void drain() { const auto count = outstanding; outstanding = 0; emit bytesWritten(count); }
protected:
    qint64 readData(char *data, qint64 maximum) override {
        const auto count = qMin(maximum, qint64(input.size()));
        memcpy(data, input.constData(), count);
        input.remove(0, count);
        return count;
    }
    qint64 writeData(const char *data, qint64 count) override {
        if (failWrites)
            return -1;
        if (writeLimit >= 0)
            count = qMin(count, writeLimit);
        output.append(data, count);
        if (buffered)
            outstanding += count;
        return count;
    }
};

static QByteArray statusJson(int count = 2, int effect = 1)
{
    QJsonArray programs;
    for (int i = 0; i < count; ++i)
        programs.append(QJsonObject{{"name", QString("Effect %1").arg(i)},
            {"description", "A test effect"}, {"colors", 1}, {"values", QJsonArray{"Speed"}}});
    return QJsonDocument(QJsonObject{{"programs", programs}, {"com", effect}, {"delay", 30},
        {"brightness", 90}, {"values", QJsonArray{1, 0, 0}},
        {"colors", QJsonArray{16711680, 65280, 255, 0, 16777215, 16711935}}}).toJson(QJsonDocument::Compact);
}

class Regression : public QObject
{
    Q_OBJECT
private slots:
    void framingAtEverySplit() {
        const QByteArray wire = "first@\r\n second\r\nthird\nfour@inside\r\n";
        for (int split = 0; split <= wire.size(); ++split) {
            MessageFramer framer;
            QList<QByteArray> messages;
            int invalid = 0;
            auto message = [&](const QByteArray &value) { messages.append(value); };
            auto bad = [&] { ++invalid; };
            framer.feed(wire.left(split), [] { return false; }, message, bad);
            framer.feed(wire.mid(split), [] { return false; }, message, bad);
            QCOMPARE(messages, (QList<QByteArray>{"first second", "third", "four@inside"}));
            QCOMPARE(invalid, 0);
        }
    }
    void literalSeparatorContext() {
        MessageFramer framer;
        QList<QByteArray> messages;
        const auto message = [&](const QByteArray &value) { messages.append(value); };
        const auto bad = [] { QFAIL("Unexpected framing error"); };
        framer.feed("@\r", [] { return true; }, message, bad);
        QVERIFY(messages.isEmpty());
        framer.feed("\nOK\r\n", [&] { return messages.isEmpty(); }, message, bad);
        QCOMPARE(messages, (QList<QByteArray>{"@", "OK"}));
        messages.clear();
        framer.feed("@\r\nnext\r\n", [] { return false; }, message, bad);
        QCOMPARE(messages, (QList<QByteArray>{"next"}));
    }
    void boundedInputAndRecovery() {
        MessageFramer framer;
        QList<QByteArray> messages;
        int invalid = 0;
        const auto feed = [&](const QByteArray &bytes) {
            framer.feed(bytes, [] { return false; },
                [&](const QByteArray &value) { messages.append(value); }, [&] { ++invalid; });
        };
        feed(QByteArray(3300, 'a') + "@\r\n\r\n");
        QCOMPARE(messages.size(), 1);
        QCOMPARE(messages[0].size(), 3300);
        feed(QByteArray(3301, 'b') + "@\nmore@\r\nend\nOK\n");
        QCOMPARE(invalid, 1);
        QCOMPARE(messages.last(), QByteArray("OK"));
        feed(QByteArray("bad\0data@\nmore\n", 15));
        QCOMPARE(invalid, 2);
        framer.reset();
        feed("partial@");
        framer.reset();
        feed("fresh\r\n");
        QCOMPARE(messages.last(), QByteArray("fresh"));
    }
    void incompleteDoesNotBecomeAReply() {
        FakeDevice device;
        ProtocolSession session;
        session.attach(&device);
        QSignalSpy replies(&session, &ProtocolSession::queryReply);
        QVERIFY(session.sendCommand(STATUS));
        device.receive("O");
        QTest::qWait(30);
        QCOMPARE(replies.size(), 0);
        QVERIFY(session.hasPendingQuery());
        device.receive("K\r\n");
        QCOMPARE(replies.size(), 1);
        QVERIFY(!session.hasPendingQuery());
    }
    void queriesSerializeAndPreserveExtraReplies() {
        FakeDevice device;
        ProtocolSession session;
        session.attach(&device);
        QSignalSpy replies(&session, &ProtocolSession::queryReply);
        QSignalSpy messages(&session, &ProtocolSession::messageReceived);
        QVERIFY(session.sendCommand(STATUS));
        QVERIFY(session.sendCommand(PIXELCOUNT));
        QCOMPARE(device.output, QByteArray("STATUS\n"));
        device.receive("boot diagnostics\r\nOK\r\n999\r\n");
        QCOMPARE(messages.size(), 3);
        QCOMPARE(replies.size(), 1);
        QCOMPARE(device.output, QByteArray("STATUS\nPIXELCOUNT\n"));
        device.receive("62\r\n");
        QCOMPARE(replies.size(), 2);
        QCOMPARE(replies[1][0].value<SERIAL_COMMAND>(), PIXELCOUNT);
        QVERIFY(!session.hasPendingQuery());
    }
    void separatorQueryAndMetadataContinuation() {
        FakeDevice device;
        ProtocolSession session;
        session.attach(&device);
        QSignalSpy replies(&session, &ProtocolSession::queryReply);
        QVERIFY(session.send("SEPARATOR"));
        QVERIFY(session.sendCommand(ALLSTATUS));
        device.receive("@\r");
        device.receive("\n");
        QCOMPARE(replies.size(), 1);
        QCOMPARE(replies[0][1].toString(), QString("@"));
        const QByteArray json = statusJson();
        for (int i = 0; i < json.size(); i += 100)
            device.receive(json.mid(i, 100) + (i + 100 < json.size() ? "@\r\n" : "\r\n"));
        QCOMPARE(replies.size(), 2);
        QCOMPARE(replies[1][1].toString().toUtf8(), json);
    }
    void updatesDoNotExpectAcknowledgement() {
        FakeDevice device;
        ProtocolSession session;
        session.attach(&device);
        QVERIFY(session.send("{\"colors\":[255]}"));
        QVERIFY(!session.hasPendingQuery());
        QVERIFY(session.sendCommand(STATUS));
        QVERIFY(session.send("{\"brightness\":20}"));
        QVERIFY(session.sendCommand(PIXELCOUNT));
        QVERIFY(!device.output.contains("brightness"));
        device.receive("OK\n");
        QCOMPARE(device.output, QByteArray("{\"colors\":[255]}\nSTATUS\n{\"brightness\":20}\nPIXELCOUNT\n"));
    }
    void failedWrites_data() {
        QTest::addColumn<int>("limit");
        QTest::newRow("failed") << -1;
        QTest::newRow("short") << 2;
        QTest::newRow("zero") << 0;
    }
    void failedWrites() {
        QFETCH(int, limit);
        FakeDevice device;
        device.failWrites = limit == -1;
        device.writeLimit = limit;
        ProtocolSession session;
        session.attach(&device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(!session.sendCommand(STATUS));
        QCOMPARE(failed.size(), 1);
        QVERIFY(!session.hasPendingQuery());
        QVERIFY(!session.sendCommand(STATUS));
    }
    void queryTimeoutInvalidatesSession() {
        FakeDevice device;
        ProtocolSession session;
        session.setTimeoutInterval(20);
        session.attach(&device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QSignalSpy replies(&session, &ProtocolSession::queryReply);
        QVERIFY(session.sendCommand(STATUS));
        QVERIFY(session.sendCommand(PIXELCOUNT));
        device.receive("O");
        QTRY_COMPARE(failed.size(), 1);
        QVERIFY(!session.hasPendingQuery());
        device.receive("K\n");
        QCOMPARE(replies.size(), 0);
        QCOMPARE(device.output, QByteArray("STATUS\n"));
        session.attach(&device);
        QVERIFY(session.sendCommand(STATUS));
        device.receive("OK\n");
        QCOMPARE(replies.size(), 1);
    }
    void blockedWriteTimeoutAndDrain() {
        FakeDevice device;
        device.buffered = true;
        ProtocolSession session;
        session.setTimeoutInterval(20);
        session.attach(&device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(session.send("{}"));
        QVERIFY(session.sendCommand(STATUS));
        QCOMPARE(device.output, QByteArray("{}\n"));
        device.drain();
        QCOMPARE(device.output, QByteArray("{}\nSTATUS\n"));
        device.drain();
        device.receive("OK\n");
        QVERIFY(!session.hasPendingQuery());
        QVERIFY(session.send("{}"));
        QTRY_COMPARE(failed.size(), 1);
    }
    void disconnectClearsPartialAndQueuedWork() {
        FakeDevice first, second;
        ProtocolSession session;
        session.attach(&first);
        QSignalSpy replies(&session, &ProtocolSession::queryReply);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(session.sendCommand(STATUS));
        QVERIFY(session.sendCommand(PIXELCOUNT));
        first.receive("O@");
        first.close();
        QCOMPARE(failed.size(), 1);
        QVERIFY(!session.hasPendingQuery());
        session.attach(&second);
        QVERIFY(session.sendCommand(STATUS));
        first.receive("K\n");
        second.receive("OK\n");
        QCOMPARE(replies.size(), 1);
        QCOMPARE(second.output, QByteArray("STATUS\n"));
    }
    void destroyedDeviceClearsSession() {
        ProtocolSession session;
        auto device = new FakeDevice;
        session.attach(device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(session.sendCommand(STATUS));
        delete device;
        QCOMPARE(failed.size(), 1);
        QVERIFY(!session.hasPendingQuery());
        QVERIFY(!session.sendCommand(STATUS));
    }
    void replyBeforeWriteDrainStillTimesOut() {
        FakeDevice device;
        device.buffered = true;
        ProtocolSession session;
        session.setTimeoutInterval(20);
        session.attach(&device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(session.sendCommand(STATUS));
        device.receive("OK\n");
        QVERIFY(!session.hasPendingQuery());
        QTRY_COMPARE(failed.size(), 1);
    }
    void queryReplyShapes() {
        QVERIFY(ProtocolSession::matchesReply(STATUS, "OK"));
        QVERIFY(!ProtocolSession::matchesReply(STATUS, "boot"));
        QVERIFY(ProtocolSession::matchesReply(BUFFERSIZE, "3300"));
        QVERIFY(ProtocolSession::matchesReply(PROGRAMCOUNT, "11"));
        QVERIFY(ProtocolSession::matchesReply(PIXELCOUNT, "62"));
        QVERIFY(!ProtocolSession::matchesReply(PIXELCOUNT, "62 LEDs"));
        QVERIFY(ProtocolSession::matchesReply(SEPARATOR, "@"));
        QVERIFY(ProtocolSession::matchesReply(PROGRAMINFO, "[]"));
        QVERIFY(!ProtocolSession::matchesReply(PROGRAMINFO, "{}"));
        QVERIFY(ProtocolSession::matchesReply(ALLSTATUS, statusJson()));
        QVERIFY(ProtocolSession::matchesReply(COLORS, "{\"colors\":[]}"));
        QVERIFY(ProtocolSession::matchesReply(VALUES, "{\"com\":1,\"values\":[]}"));
        QVERIFY(ProtocolSession::matchesReply(VALUES_COLORS, "{\"com\":1,\"colors\":[]}"));
        QVERIFY(ProtocolSession::matchesReply(HARDWARE, "{\"type\":\"APA102\",\"pixelcount\":62}"));
        QVERIFY(!ProtocolSession::matchesReply(HARDWARE, "{\"colors\":[]}"));
    }
    void invalidRepliesAndQueueBounds() {
        FakeDevice device;
        ProtocolSession session;
        session.attach(&device);
        QSignalSpy failed(&session, &ProtocolSession::sessionFailed);
        QVERIFY(!session.sendCommand(INVALID));
        QVERIFY(!session.sendCommand(COMMAND_COUNT));
        QVERIFY(!session.send("UNKNOWN"));
        QVERIFY(!session.send("STATUS\nPIXELCOUNT"));
        QVERIFY(!session.send(QString(3301, 'a')));
        QVERIFY(session.sendCommand(STATUS));
        for (int i = 0; i < 32; ++i)
            QVERIFY(session.sendCommand(STATUS));
        QVERIFY(!session.sendCommand(STATUS));
        device.receive(QByteArray(3301, 'x') + "\n");
        QCOMPARE(failed.size(), 1);
        QVERIFY(!session.hasPendingQuery());
    }
    void metadataDefinesEffectRange() {
        SerialStatus status;
        QVERIFY(status.parse(statusJson(2, 1)));
        QCOMPARE(status.m_programs.size(), 2);
        QVERIFY(!status.parse(statusJson(2, 2)));
        QVERIFY(!status.isValid());
        QVERIFY(status.parse(statusJson(18, 17))); // more effects than commands
        QCOMPARE(status.m_com, 17);
        QVERIFY(!status.parse(statusJson(0, 0)));
        auto object = QJsonDocument::fromJson(statusJson()).object();
        auto programs = object["programs"].toArray();
        programs[0] = QJsonObject{{"name", "broken"}};
        object["programs"] = programs;
        QVERIFY(!status.parse(QJsonDocument(object).toJson()));
        object = QJsonDocument::fromJson(statusJson()).object();
        object["colors"] = QJsonArray{1};
        QVERIFY(!status.parse(QJsonDocument(object).toJson()));
        QVERIFY(!status.parse("[]"));
        QVERIFY(!status.parse("garbage"));
    }
    void usbGuiWorkflowWithEmulatedDevice() {
        int master = -1, slave = -1;
        char name[256] = {};
        QVERIFY(openpty(&master, &slave, name, nullptr, nullptr) == 0);
        struct CloseFd { int fd; ~CloseFd() { if (fd >= 0) close(fd); } } closeMaster{master}, closeSlave{slave};
        QByteArray requests;
        QSocketNotifier notifier(master, QSocketNotifier::Read);
        connect(&notifier, &QSocketNotifier::activated, this, [&](QSocketDescriptor, QSocketNotifier::Type) {
            char bytes[4096];
            const auto length = read(master, bytes, sizeof(bytes));
            if (length > 0)
                requests.append(bytes, length);
        });
        MainWindow window;
        window.show();
        auto app = LinuxStripApp::instance();
        auto &transport = app->m_Transport;
        auto tabs = window.findChild<QTabWidget *>("tabWidget");
        auto programs = window.findChild<FormPrograms *>();
        QVERIFY(programs);
        QVERIFY(!programs->isEnabled());
        QVERIFY(transport.connectToPort(QString::fromLocal8Bit(name)));
        QCOMPARE(transport.state(), SerialTransport::State::Connecting);
        tabs->setCurrentIndex(1); // ALLSTATUS arrives while Manual is visible.
        QTRY_VERIFY_WITH_TIMEOUT(requests.contains("ALLSTATUS\n"), 3000);
        QCOMPARE(transport.state(), SerialTransport::State::Connected);
        const QByteArray status = statusJson() + "\r\n";
        QCOMPARE(write(master, status.constData(), status.size()), ssize_t(status.size()));
        QTRY_VERIFY(programs->isEnabled());
        auto effects = window.findChild<QComboBox *>("comboProgramName");
        QCOMPARE(effects->count(), 2);
        QCOMPARE(effects->currentIndex(), 1);
        QTest::qWait(180);
        QCOMPARE(requests, QByteArray("ALLSTATUS\n")); // loading never writes controls
        auto commands = window.findChild<QComboBox *>("comboCommands");
        QCOMPARE(commands->count(), int(COMMAND_COUNT) - 1);
        commands->setCurrentText("SEPARATOR");
        QTest::mouseClick(window.findChild<QPushButton *>("btnSendCommand"), Qt::LeftButton);
        QTRY_VERIFY(requests.contains("SEPARATOR\n"));
        tabs->setCurrentIndex(0); // manual query completes while Programs is visible.
        QCOMPARE(write(master, "@\r\n", 3), ssize_t(3));
        auto log = window.findChild<QTextEdit *>("textEditResponse");
        QTRY_VERIFY(log->toPlainText().trimmed().endsWith("@"));
        effects->setCurrentIndex(0);
        QTRY_VERIFY(requests.contains("\"com\":0"));
        window.findChild<QSpinBox *>("spinBoxBrightness")->setValue(77);
        transport.disconnectDevice(); // cancel a queued/debounced control update
        const QByteArray atDisconnect = requests;
        QTest::qWait(150);
        QCOMPARE(requests, atDisconnect);
        QCOMPARE(transport.state(), SerialTransport::State::Disconnected);
        QVERIFY(!programs->isEnabled());
        QVERIFY(transport.connectToPort(QString::fromLocal8Bit(name)));
        transport.disconnectDevice(); // cancel during the USB boot interval
        QTest::qWait(1600);
        QCOMPARE(transport.state(), SerialTransport::State::Disconnected);
        QCOMPARE(requests, atDisconnect);
        requests.clear();
        QVERIFY(transport.connectToPort(QString::fromLocal8Bit(name)));
        QTRY_VERIFY_WITH_TIMEOUT(requests.contains("ALLSTATUS\n"), 3000);
        QCOMPARE(write(master, status.constData(), status.size()), ssize_t(status.size()));
        QTRY_VERIFY(programs->isEnabled());
        notifier.setEnabled(false);
        close(master);
        closeMaster.fd = -1;
        close(slave);
        closeSlave.fd = -1;
        QTRY_COMPARE(transport.state(), SerialTransport::State::Disconnected);
        QVERIFY(!programs->isEnabled());
        QVERIFY(window.findChild<QLabel *>("labelX")->text().contains("error", Qt::CaseInsensitive));
        transport.connectToBluetooth("invalid");
        QCOMPARE(transport.state(), SerialTransport::State::Disconnected);
        QVERIFY(!transport.connectToPort("/dev/linstrip-nonexistent-device"));
        QCOMPARE(transport.state(), SerialTransport::State::Disconnected);
    }
};

int main(int argc, char **argv)
{
    LinuxStripApp app(argc, argv);
    Regression tests;
    return QTest::qExec(&tests, argc, argv);
}
#include "regression.moc"
