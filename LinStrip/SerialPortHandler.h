#ifndef SERIALPORTHANDLER_H
#define SERIALPORTHANDLER_H


#include <QtSerialPort/QSerialPort>
#include <QSerialPortInfo>
#include <QByteArray>
#include <QTextStream>
#include <QTextEdit>
#include <QTimer>
#include <QList>
#include "serialcommands.h"



QT_BEGIN_NAMESPACE

    QT_END_NAMESPACE




class SerialPortHandler : public QObject
{
Q_OBJECT

public:
    explicit SerialPortHandler( QSerialPort *serialPort, QObject *parent = nullptr );
    bool send( const char *strToSend );
    void stopTimer();
    qint64 write( const QByteArray &writeData );
    static QStringList getAvailablePorts()
    {
        QStringList portsNames;
        auto available = QSerialPortInfo::availablePorts();

        for( const QSerialPortInfo &info : available )
            portsNames.append( info.portName() );

        return portsNames;
    }

    void setEdit( QTextEdit *pTextEdit );
    bool m_debugging = false;

signals:
    void dataHasCome( QString str );

private slots:
    void handleReadyRead();
    void handleTimeout();
    void handleError( QSerialPort::SerialPortError error );

private:
    //QSerialPort *m_serialPort = nullptr;
    QSerialPort *m_serialPort;
    QByteArray m_writeData;
    QByteArray m_readData;
    QTextStream m_standardOutput;
    QTimer m_timer;
    QTextEdit *m_pTextEdit = nullptr;

};

#endif
