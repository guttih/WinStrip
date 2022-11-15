#include <QTimer>
#include <QFile>
#include "LinuxStripApp.h"

LinuxStripApp::LinuxStripApp( int &argc, char ** argv ) : QApplication( argc, argv )
{
    connect( &m_ReadFileTimer, &QTimer::timeout, this, &LinuxStripApp::CheckAndGetExternalCommands );
    m_ReadFileTimer.start( 2000 );
    this->m_ApplicationInformation.workingDirectory = QApplication::applicationDirPath();
    this->m_ApplicationInformation.fileName = QApplication::applicationFilePath();
    this->m_ApplicationInformation.inputFileName = m_ApplicationInformation.fileName + ".SendToSerial";
}


void LinuxStripApp::CheckAndGetExternalCommands()
{
    if( !m_ApplicationInformation.pullExternalCommands || !m_Transport.isConnected() )
        return;

    QFile file( this->m_ApplicationInformation.inputFileName );
    if(  file.open( QIODevice::ReadWrite ) )
    {
        QByteArray bytes = file.readAll();
        if( bytes.length() > 0 )
        {
            file.resize( 0 );
            file.close();


            bool success;
            QString str = bytes;
            QStringList lst = str.split( "\n", Qt::SkipEmptyParts );

            QTextStream output( stdout );
            output << "Sending to Serial: ";
            for( const auto &str : qAsConst( lst ) )
            {
                success = m_Transport.send( str.toStdString().c_str() );
                output << QObject::tr( "%0 %1" ).arg( str ).arg( success ? " (Sent)" : " (Error sending)" ) << Qt::endl;
            }
        }
    }

}

LinuxStripApp::~LinuxStripApp()
{

}

LinuxStripApp *LinuxStripApp::instance()
{
    return ( LinuxStripApp* ) QApplication::instance();
}

