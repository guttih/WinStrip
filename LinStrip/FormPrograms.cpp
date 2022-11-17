#include <QColorDialog>
#include <QDesktopServices>
#include "FormPrograms.h"
#include "ui_FormPrograms.h"

FormPrograms::FormPrograms( QWidget *parent ) :
    QWidget( parent ),
    ui( new Ui::FormPrograms )
{
    ui->setupUi( this );
    m_pApplication = LinuxStripApp::instance();

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
    QColor color = QColorDialog::getColor( btnColor->palette().button().color(), this );
    if( color.isValid() )
    {
        ColorToFormButton( btnColor, color.name().right( 6 ) );
    }
}

void FormPrograms::ColorToFormButton( QPushButton *btnColor, QString strHexidecimalColor )
{

    QString strStyle = QString( "QPushButton { background-color: grey; }\n"
                                "QPushButton:enabled { background-color: #%0; }\n" ).arg( strHexidecimalColor );
    btnColor->setStyleSheet( strStyle );
    btnColor->setText( QString( "%0" ).arg( strHexidecimalColor ) );
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
    while( hexadecimal.length() < 6 )
        hexadecimal.insert( 0, "0" );
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

void FormPrograms::on_sliderDelay_valueChanged( int value )
{
    ui->spinBoxDelay->setValue( value );
}

void FormPrograms::on_sliderBrightness_valueChanged( int value )
{
    ui->spinBoxBrightness->setValue( value );
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

void FormPrograms::on_spinBoxDelay_valueChanged( int value )
{
    ui->sliderDelay->setValue( value );
}

void FormPrograms::on_spinBoxBrightness_valueChanged( int value )
{
    ui->sliderBrightness->setValue( value );
}

bool FormPrograms::ProgramsToForm( QString programJsonList  )
{
    m_ProgramList = m_pApplication->m_Transport.parseJsonProgramInformation( programJsonList );
    ui->comboProgramName->clear();
    for( const auto program: qAsConst( m_ProgramList ) )
    {
        ui->comboProgramName->addItem( program->m_name );
    }

    return m_ProgramList.count() > 0;

}

void FormPrograms::SerialStatusToForm()
{

    //Set delay and brightness to form
    ui->spinBoxDelay->setValue( m_SerialStatus.m_delay );
    ui->spinBoxBrightness->setValue( m_SerialStatus.m_brightness );

    //Set all values to form
    ui->spinBox0->setValue( m_SerialStatus.m_values[ 0 ] );
    ui->spinBox1->setValue( m_SerialStatus.m_values[ 1 ] );
    ui->spinBox2->setValue( m_SerialStatus.m_values[ 2 ] );

    //Set all colors on form buttons
    ColorToFormButton( ui->btnColor0, DecToHex( m_SerialStatus.m_colors[ 0 ] ) );
    ColorToFormButton( ui->btnColor2, DecToHex( m_SerialStatus.m_colors[ 2 ] ) );
    ColorToFormButton( ui->btnColor1, DecToHex( m_SerialStatus.m_colors[ 1 ] ) );
    ColorToFormButton( ui->btnColor3, DecToHex( m_SerialStatus.m_colors[ 3 ] ) );
    ColorToFormButton( ui->btnColor4, DecToHex( m_SerialStatus.m_colors[ 4 ] ) );
    ColorToFormButton( ui->btnColor5, DecToHex( m_SerialStatus.m_colors[ 5 ] ) );

    //Add Program names to combobox
    ui->comboProgramName->clear();
    for( const auto program: qAsConst( m_SerialStatus.m_programs ) )
    {
        ui->comboProgramName->addItem( program->m_name );
    }

    //Select currently running program
    ui->comboProgramName->setCurrentIndex( m_SerialStatus.m_com );

}

bool FormPrograms::AllStatusToForm( QString jsonAllStatus  )
{
    if( !m_SerialStatus.parse( jsonAllStatus ) )
        return false;
    SerialStatusToForm();
    return true;
}

void FormPrograms::on_comboProgramName_currentIndexChanged( int index )
{
    if( index > -1 && index < m_SerialStatus.m_programs.count() )
    {
        ui->textEditProgramDesc->setText( m_SerialStatus.m_programs.at( index )->m_description );

        int valueCount = m_SerialStatus.m_programs.at( index )->m_values.count();
        if( valueCount > 0 )
        {
            //ui->hGroupBox0->show();
            ui->hGroupBox0->setTitle( m_SerialStatus.m_programs.at( index )->m_values.at( 0 ) );
        }
        else
            ui->hGroupBox0->setTitle( "Value 0" );
        if( valueCount > 1 )
        {
            //ui->hGroupBox1->show();
            ui->hGroupBox1->setTitle( m_SerialStatus.m_programs.at( index )->m_values.at( 1 ) );
        }
        else
            ui->hGroupBox1->setTitle( "Value 1" );
        if( valueCount > 2 )
        {
            //ui->hGroupBox2->show();
            ui->hGroupBox2->setTitle( m_SerialStatus.m_programs.at( index )->m_values.at( 2 ) );
        }
        else
            ui->hGroupBox2->setTitle( "Value 2" );

        ui->hGroupBox0->setDisabled( valueCount < 1 );
        ui->hGroupBox1->setDisabled( valueCount < 2 );
        ui->hGroupBox2->setDisabled( valueCount < 3 );
    }
}

