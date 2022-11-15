#include <QColorDialog>
#include <QDesktopServices>
#include "FormPrograms.h"
#include "ui_FormPrograms.h"

FormPrograms::FormPrograms( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormPrograms )
{
    ui->setupUi( this );

    int max = 2000;
    ui->hSlider0->setMaximum( max );
    ui->hSlider1->setMaximum( max );
    ui->hSlider2->setMaximum( max );
    ui->spinBox0->setMaximum( max );
    ui->spinBox1->setMaximum( max );
    ui->spinBox2->setMaximum( max );
}
void FormPrograms::showEvent( QShowEvent* event )
{
    QWidget::showEvent( event );

    ui->btnColor0->setStyleSheet( "QPushButton { background-color: grey; }\n"
                                  "QPushButton:enabled { background-color: rgb(200,0,0); }\n" );
}

FormPrograms::~FormPrograms()
{
    delete ui;
}

void FormPrograms::btnColor_clicked( QPushButton *btnColor )
{
    QColor color = QColorDialog::getColor( Qt::yellow, this );
    if( color.isValid() )
    {
        qDebug() << "Color Choosen : " << color.name();
        QString str = QString( "QPushButton { background-color: grey; }\n"
                               "QPushButton:enabled { background-color: %0; }\n" ).arg( color.name() );
        btnColor->setStyleSheet( str );
        btnColor->setText( QString( "%0" ).arg( color.name() ) );
    }
}
uint FormPrograms::HexToDec( QString hexString )
{
    int i = hexString.indexOf( "#" );
    if( i == 0 )
        i += 1;
    else if( i == -1 )
    {
        i = hexString.indexOf( "0x" );
        if( i > -1 )
            i += 2;
    }
    if( i == -1 )
        i = 0;

    bool ok;
    QString col = hexString.right( hexString.length() - i );
    uint decNum = col.toUInt( &ok, 16 );
    return decNum;
}

QString FormPrograms::DecToHex( int number )
{
    QString hexadecimal;
    hexadecimal.setNum( number, 16 );
    return hexadecimal;
}
void FormPrograms::on_btnColor0_clicked()
{
    btnColor_clicked( ui->btnColor0 );
}

void FormPrograms::on_btnColor1_clicked()
{
    btnColor_clicked( ui->btnColor1 );
}


void FormPrograms::on_btnColor2_clicked()
{
    btnColor_clicked( ui->btnColor2 );
}


void FormPrograms::on_btnColor3_clicked()
{
    btnColor_clicked( ui->btnColor3 );
}


void FormPrograms::on_btnColor4_clicked()
{
    btnColor_clicked( ui->btnColor4 );
}


void FormPrograms::on_btnColor5_clicked()
{
    btnColor_clicked( ui->btnColor5 );
}


void FormPrograms::on_commandLinkButton_clicked()
{
    QDesktopServices::openUrl( QUrl( "https://guttih.com/list/project-winstrip" ) );
}


void FormPrograms::on_hSlider0_valueChanged( int value )
{
    ui->spinBox0->setValue( value );
}

void FormPrograms::on_hSlider1_valueChanged( int value )
{
    ui->spinBox1->setValue( value );
}


void FormPrograms::on_hSlider2_valueChanged( int value )
{
    ui->spinBox2->setValue( value );
}

void FormPrograms::on_spinBox0_valueChanged( int value )
{
    ui->hSlider0->setValue( value );
}

void FormPrograms::on_spinBox1_valueChanged( int value )
{
    ui->hSlider1->setValue( value );
}


void FormPrograms::on_spinBox2_valueChanged( int value )
{
    ui->hSlider2->setValue( value );
}

