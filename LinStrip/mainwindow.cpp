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
    // m_formCommmands->setMainForm( this );
    ui->tabWidget->addTab( m_formCommmands, QString( "Manual" ) );
    ui->tabWidget->setCurrentIndex( 2 );

    for( const auto &deviceName : SerialPortHandler::getAvailablePorts() )
        ui->comboDevices->addItem( deviceName );

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
            m_pApplication->m_Transport.sendCommand( SERIAL_COMMAND::PROGRAMINFO );


    }
    ui->btnConnect->setText( isConnected ? "Disconnect" : "Connect" );
}
void MainWindow::on_newData( QString str )
{
    int i = 0;
    i++;

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

