#ifndef _SERIALTRANSPORT_H
#define _SERIALTRANSPORT_H

#include "SerialPortHandler.h"

class SerialTransport : public QObject
{
Q_OBJECT

public:
    typedef void (*onnewdata_t)( QString );
    SerialTransport();
    virtual ~SerialTransport();
    QSerialPort *getSerialPort();
    SerialPortHandler *setSerialHandler( SerialPortHandler *pSerialPortHandler );
    SerialPortHandler *getSerialHandler();
    bool connectToPort( const QString &name, int baudRate, QObject *parent );
    bool isConnected();
    bool send( const char *strToSend );
    bool sendCommand( SERIAL_COMMAND serialCommand );
    void registerOnNewData( onnewdata_t func );

signals:
    void dataHasCome( QString str );

private slots:
    void dataFromHandler( QString str );

private:
    std::vector< onnewdata_t >    funcs;
    QSerialPort m_serialPort;
    SerialPortHandler *m_SerialHandler = nullptr;
};

#endif
