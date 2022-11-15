#ifndef SERIALTRANSPORT_H
#define SERIALTRANSPORT_H

// #include <QtSerialPort/QSerialPort>
#include "SerialPortHandler.h"

class SerialTransport
{
public:
    SerialTransport();
    ~SerialTransport();
    QSerialPort *getSerialPort();
    SerialPortHandler *setSerialHandler( SerialPortHandler *pSerialPortHandler );
    SerialPortHandler *getSerialHandler();
    bool connectToPort( const QString &name, int baudRate, QObject *parent );
    bool isConnected();
    bool send( const char *strToSend );
    bool sendCommand( SERIAL_COMMAND serialCommand );

private:
    QSerialPort m_serialPort;
    SerialPortHandler *m_SerialHandler = nullptr;
};

#endif
