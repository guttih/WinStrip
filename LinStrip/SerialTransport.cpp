#include "SerialTransport.h"

SerialTransport::SerialTransport()
{

}

SerialTransport::~SerialTransport()
{

}

void SerialTransport::registerOnNewData( onnewdata_t func )
{
    funcs.push_back( func );
}
void SerialTransport::dataFromHandler( QString str )
{
    QTextStream output( stdout );
    output << "SerialTransport::dataFromHandler got: ";
    output << QObject::tr( "%0" ).arg( str ) << Qt::endl;
    emit dataHasCome( str );
}

SerialPortHandler *SerialTransport::getSerialHandler()
{
    return this->m_SerialHandler;
}
SerialPortHandler *SerialTransport::setSerialHandler( SerialPortHandler *pSerialPortHandler )
{
    m_SerialHandler = pSerialPortHandler;
    connect( m_SerialHandler, SIGNAL( dataHasCome( QString ) ), this, SLOT( dataFromHandler( QString ) ) );
    return this->getSerialHandler();
}
QSerialPort *SerialTransport::getSerialPort()
{
    QSerialPort *pD = &m_serialPort;
    return pD;
}

bool SerialTransport::isConnected()
{
    return m_serialPort.isOpen();
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
bool SerialTransport::send( const char *strToSend )
{
    auto serialHandler = this->getSerialHandler();
    if( serialHandler )
    {
        QString str( strToSend );
        if( str.length() > 0 )
            return serialHandler->send( str.toStdString().c_str() );
    }
    return false;
}

bool SerialTransport::sendCommand( SERIAL_COMMAND serialCommand )
{
    if( m_LastCommand != SERIAL_COMMAND::INVALID )
        return false; //you need to process value from last command
    m_LastCommand = serialCommand;
    return send( SERIAL_COMMAND_STRING[ serialCommand ] );
}

void SerialTransport::clearLastCommand()
{
    m_LastCommand = SERIAL_COMMAND::INVALID;
}

SERIAL_COMMAND SerialTransport::getLastCommand()
{
    return m_LastCommand;
};

QList< SerialProgramInformation * > SerialTransport::parseJsonProgramInformation( QString str )
{
    QList< SerialProgramInformation * > list;

    JsonG::Json json( str.toStdString().c_str() );

    if( !json.isValid() )
        return list;

    JsonData *root=json.getRootObject();
    if( !root || root->getType() != JSONTYPE::JSONTYPE_ARRAY )
        return list;

    JsonData *child=root->getChildAt( 0 );
    SerialProgramInformation spi;
    while( child )
    {
        spi.parseJsonProgramInformation( child );
        if( spi.isValid() )
        {
            list.append( new SerialProgramInformation( spi ) );
        }
        child=child->getNext();
    }

    return list;
}