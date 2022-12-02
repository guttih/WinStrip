#include "SerialPortHandler.h"
#include <QCoreApplication>
#include "Json.h"

SerialPortHandler::SerialPortHandler( QSerialPort *serialPort, QObject *parent ) :
    QObject( parent ),
    m_serialPort( serialPort ),
    m_standardOutput( stdout )
{

    connect( m_serialPort, &QSerialPort::readyRead, this, &SerialPortHandler::handleReadyRead );
    connect( m_serialPort, &QSerialPort::errorOccurred, this, &SerialPortHandler::handleError );
    connect( &m_timer, &QTimer::timeout, this, &SerialPortHandler::handleTimeout );

    m_timer.start( 1000 );
}

void SerialPortHandler::stopTimer()
{
    disconnect( m_serialPort, &QSerialPort::readyRead, this, &SerialPortHandler::handleReadyRead );
    disconnect( m_serialPort, &QSerialPort::errorOccurred, this, &SerialPortHandler::handleError );
    disconnect( &m_timer, &QTimer::timeout, this, &SerialPortHandler::handleTimeout );

    m_timer.stop();
}


qint64 SerialPortHandler::write( const QByteArray &writeData )
{
    m_writeData = writeData;
    m_timer.stop();
    const qint64 bytesWritten = m_serialPort->write( writeData );

    if( bytesWritten == -1 )
    {
        m_standardOutput << QObject::tr( "Failed to write the data to port %1, error: %2" )
        .arg( m_serialPort->portName() )
        .arg( m_serialPort->errorString() )
        << Qt::endl;

    }
    else if( bytesWritten != m_writeData.size() )
    {
        m_standardOutput << QObject::tr( "Failed to write all the data to port %1, error: %2" )
        .arg( m_serialPort->portName() )
        .arg( m_serialPort->errorString() )
        << Qt::endl;

    }

    m_timer.start( 1000 );
    return bytesWritten;

}

bool SerialPortHandler::send( const char *strToSend )
{
    QByteArray ba( strToSend );
    auto ret = this->write( ba + "\n" );
    m_standardOutput << QObject::tr( "Sent command \"%1\", %2 bytes" ).arg( ba ).arg( ret ) << Qt::endl;
    return ret > 0;

}

void SerialPortHandler::handleReadyRead()
{
    m_readData.append( m_serialPort->readAll() );
    //m_serialPort->clear(QSerialPort::Direction::Input);


    if( !m_timer.isActive() )
    {
        m_timer.start( 1000 );

    }
}

void SerialPortHandler::handleTimeout()
{
    if( m_readData.isEmpty() )
    {
        if( m_debugging )
        {
            m_standardOutput << QObject::tr( "No data was currently available "
                                             "for reading from port %1" )
            .arg( m_serialPort->portName() )
            << Qt::endl;
        }
    }
    else
    {
        m_standardOutput << QObject::tr( "Data successfully received from port %1" )
        .arg( m_serialPort->portName() )
        << Qt::endl;
        QString str = QString::fromStdString( m_readData.toStdString() );
        int i;
        while( ( i=str.indexOf( "@\r\n" ) ) > -1 )
        {
            str=str.remove( i, 3 );
        }
        m_readData.clear();
        m_standardOutput << str << Qt::endl;

        if( m_pTextEdit )
            m_pTextEdit->append( str );

        emit dataHasCome( str );

        m_serialPort->clear();
    }

}



void SerialPortHandler::handleError( QSerialPort::SerialPortError serialPortError )
{
    if( serialPortError == QSerialPort::ReadError )
    {
        m_standardOutput << QObject::tr( "An I/O error occurred while reading "
                                         "the data from port %1, error: %2" )
        .arg( m_serialPort->portName() )
        .arg( m_serialPort->errorString() )
        << Qt::endl;
        QCoreApplication::exit( 1 );
    }
}


