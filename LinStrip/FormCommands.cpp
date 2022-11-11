#include "FormCommands.h"
#include "ui_FormCommands.h"
#include "./mainwindow.h"


FormCommands::FormCommands( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormCommands )
{
    ui->setupUi( this );
    for( auto command : SERIAL_COMMAND_STRING )
        ui->comboCommands->addItem( QString::fromUtf8( command ) );

    ui->textEditResponse->ensureCursorVisible();

}

FormCommands::~FormCommands()
{
    delete ui;
}

void FormCommands::showEvent( QShowEvent* event )
{
    QWidget::showEvent( event );
    bool b =false;
    b=true;
}

void FormCommands::hideEvent( QHideEvent* event )
{
    QWidget::hideEvent( event );
    bool b =false;
    b=true;
}

QTextEdit *FormCommands::GetTextEditResponce()
{
    return ui->textEditResponse;
}


void FormCommands::on_btnSendCommand_clicked()
{

    MainWindow* mainWindow = ( MainWindow* ) this->m_mainWindow;
    SerialPortHandler *ph = mainWindow->getSerialHandler();
    if( mainWindow && ph )
    {
        QString str = ui->comboCommands->currentText();
        if( str.length() > 0 )
            /*QWidget *pw = this->parentWidget();
            QObject *po = this->parent();
            po = po->parent();*/
            ph->send( str.toStdString().c_str() );
    }


}

void FormCommands::setMainForm( QWidget *mainWindow )
{
    this->m_mainWindow = mainWindow;
}


void FormCommands::on_pushButton_clicked()
{
    ui->textEditResponse->clear();
}

