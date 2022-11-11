#include "FormCommands.h"
#include "ui_FormCommands.h"

FormCommands::FormCommands( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormCommands )
{
    ui->setupUi( this );
    /*for( int i = 0; i < SERIAL_COMMAND::COMMAND_COUNT; i++ )
    {
        ui->comboCommands->addItem( QString::fromUtf8( SERIAL_COMMAND_STRING[ i ] ) );
    }*/

    for( auto command : SERIAL_COMMAND_STRING )
        ui->comboCommands->addItem( QString::fromUtf8( command ) );


    for( const auto &deviceName : SerialPortHandler::getAvailablePorts() )
        ui->comboDevices->addItem( deviceName );

}

FormCommands::~FormCommands()
{
    delete ui;
}

void FormCommands::on_FormCommands_windowTitleChanged( const QString &title )
{

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

