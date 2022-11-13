#include "SerialTransport.h"

SerialTransport::SerialTransport()
{

}

SerialTransport::~SerialTransport()
{

}
SerialPortHandler *SerialTransport::getSerialHandler()
{
    return this->m_SerialHandler;
}
SerialPortHandler *SerialTransport::setSerialHandler( SerialPortHandler *pSerialPortHandler )
{
    m_SerialHandler = pSerialPortHandler;
    return this->getSerialHandler();
}
QSerialPort *SerialTransport::getSerialPort()
{
    QSerialPort *pD = &m_serialPort;
    return pD;
}

bool SerialTransport::connectToPort( const QString &name, int baudRate, QObject *parent )
{

    auto serialPort = getSerialPort();
    auto serialHandler = getSerialHandler();
    if( serialPort->isOpen() )
    {
        serialHandler->stopTimer();
        serialPort->close();
    }

    serialPort->setPortName( name );
    serialPort->setBaudRate( baudRate );
    bool success = serialPort->open( QIODevice::ReadWrite );
    if( !success )
    {
        // QMessageBox msgBox;
        // msgBox.critical( this, "Error connecting", QString( "error %1, %2" ).arg( name, serialPort->errorString() ) );
        //todo: how to report error to Gui.
        return false;
    }

    if( serialHandler )
    {
        delete serialHandler;
        serialHandler = nullptr;
    }
    serialHandler = setSerialHandler( new SerialPortHandler( serialPort, parent ) );

    //todo: hér ætti að tengja við eitthvað update stuff
    return success;

}