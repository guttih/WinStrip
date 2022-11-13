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

private:
    QSerialPort m_serialPort;
    SerialPortHandler *m_SerialHandler = nullptr;
};

#endif