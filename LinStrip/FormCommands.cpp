#include "FormCommands.h"
#include "ui_FormCommands.h"
#include "./mainwindow.h"


FormCommands::FormCommands( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormCommands )
{
    ui->setupUi( this );

    m_pApplication = LinuxStripApp::instance();

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
    QString str = ui->comboCommands->currentText();
    if( str.length() > 0 )
        m_pApplication->m_Transport.send( str.toStdString().c_str() );
}

void FormCommands::on_pushButton_clicked()
{
    ui->textEditResponse->clear();
}

