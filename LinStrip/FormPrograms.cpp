#include "FormPrograms.h"
#include "ui_FormPrograms.h"

FormPrograms::FormPrograms( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormPrograms )
{
    ui->setupUi( this );
}

FormPrograms::~FormPrograms()
{
    delete ui;
}
