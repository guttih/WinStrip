#include "formcommands.h"
#include "ui_formcommands.h"

FormCommands::FormCommands( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormCommands )
{
    ui->setupUi( this );
    for( int i = 0; i < SERIAL_COMMAND::COMMAND_COUNT; i++ )
    {
        ui->comboCommands->addItem( QString::fromUtf8( SERIAL_COMMAND_STRING[ i ] ) );
    }
}

FormCommands::~FormCommands()
{
    delete ui;
}
