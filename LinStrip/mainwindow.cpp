#include <QMessageBox>
#include <QFile>
#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow( QWidget *parent )
    : QMainWindow( parent )
    , ui( new Ui::MainWindow )
{
    ui->setupUi( this );

    m_pApplication = LinuxStripApp::instance();

    m_formCommmands = new FormCommands( this );
    m_formPrograms = new FormPrograms( this );

    // m_formCommmands->setMainForm( this );
    ui->tabWidget->addTab( m_formPrograms, QString( "Programs" ) );
    ui->tabWidget->addTab( m_formCommmands, QString( "Manual" ) );

    ui->tabWidget->setCurrentIndex( 1 );

    for( const auto &deviceName : SerialPortHandler::getAvailablePorts() )
        ui->comboDevices->addItem( deviceName );
    QObject::connect( &m_pApplication->m_Transport, SIGNAL( dataHasCome( QString ) ), this, SLOT( on_newData( QString ) ) );

}

MainWindow::~MainWindow()
{
    delete ui;
}


void MainWindow::on_action_About_2_triggered()
{
    QMessageBox::about( this, "About", "\nLinStrip\n    Version 1.0     \n" );
}


void MainWindow::on_MainWindow_iconSizeChanged( const QSize &iconSize )
{

}

void MainWindow::on_tabWidget_tabBarClicked( int index )
{
    int oldIndex =  ui->tabWidget->currentIndex();
    if( oldIndex == index )
    {
        return; //clicked tab is the same as the visable tab
    }

    // Switching tabs
}

void MainWindow::on_btnConnect_clicked()
{
    bool isConnected = ui->btnConnect->text() != "Connect";

    if( isConnected )
    {
        m_pApplication->m_Transport.getSerialHandler()->stopTimer();
        m_pApplication->m_Transport.getSerialPort()->close();
        isConnected=false;
    }
    else
    {
        QString portName = ui->comboDevices->currentText();
        if( portName.length() < 1 )
            return;
        if( connectToPort( portName, 115200 ) )
        {
            m_pApplication->m_Transport.sendCommand( SERIAL_COMMAND::ALLSTATUS );
            isConnected=true;
        }


    }
    ui->btnConnect->setText( isConnected ? "Disconnect" : "Connect" );
}
void MainWindow::on_newData( QString str )
{
    //qDebug() << "MainWindow::on_newData : " << str;
    processCommandResponse( str );

}


bool MainWindow::connectToPort( const QString &name, int baudRate )
{
    bool success = m_pApplication->m_Transport.connectToPort( name,  baudRate, this );
    if( success )
    {
        if( ui->tabWidget->currentIndex() == 2 )
            m_pApplication->m_Transport.getSerialHandler()->setEdit( m_formCommmands->GetTextEditResponce() );
    }
    else
    {
        QMessageBox msgBox;
        msgBox.critical( this, "Error connecting", QString( "error %1, %2" ).arg( name, m_pApplication->m_Transport.getSerialPort()->errorString() ) );
    }
    return success;
}


bool MainWindow::processCommandResponse( QString response )
{
    SERIAL_COMMAND command = m_pApplication->m_Transport.getLastCommand();
    if( command == SERIAL_COMMAND::INVALID )
        return false;

    //resetting last command
    m_pApplication->m_Transport.clearLastCommand();

    switch( command )
    {
        case PROGRAMINFO:
            return m_formPrograms->ProgramsToForm( response );
            break;
        case ALLSTATUS:
            m_formPrograms->AllStatusToForm( response );
            return true;
            break;
    }

    return false;

}
