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

    connect( &m_timerSend, &QTimer::timeout, this, &FormPrograms::on_TimerSend );
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
    SendFormColorsToSerial();
}

void FormPrograms::on_btnColor1_clicked()
{
    btnColor_clicked( ui->btnColor1 );
    SendFormColorsToSerial();
}


void FormPrograms::on_btnColor2_clicked()
{
    btnColor_clicked( ui->btnColor2 );
    SendFormColorsToSerial();
}


void FormPrograms::on_btnColor3_clicked()
{
    btnColor_clicked( ui->btnColor3 );
    SendFormColorsToSerial();
}


void FormPrograms::on_btnColor4_clicked()
{
    btnColor_clicked( ui->btnColor4 );
    SendFormColorsToSerial();
}


void FormPrograms::on_btnColor5_clicked()
{
    btnColor_clicked( ui->btnColor5 );
    SendFormColorsToSerial();
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
    SendFormValuesToSerial();
}

void FormPrograms::on_spinBox1_valueChanged( int value )
{
    ui->hSlider1->setValue( value );
    SendFormValuesToSerial();
}


void FormPrograms::on_spinBox2_valueChanged( int value )
{
    ui->hSlider2->setValue( value );
    SendFormValuesToSerial();
}

void FormPrograms::on_spinBoxDelay_valueChanged( int value )
{
    ui->sliderDelay->setValue( value );
    SendFormValuesToSerial();
}

void FormPrograms::on_spinBoxBrightness_valueChanged( int value )
{
    ui->sliderBrightness->setValue( value );
    SendFormValuesToSerial();
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
    SendFormValuesToSerial();
}


void FormPrograms::SendFormValuesToSerial()
{
    if( m_timerSend.isActive() )
        m_timerSend.stop();



    int delay = ui->spinBoxDelay->value();
    int com = ui->comboProgramName->currentIndex();
    int brightness = ui->spinBoxBrightness->value();
    int value0 = ui->hGroupBox0->isEnabled() ? ui->spinBox0->value() : 0;
    int value1 = ui->hGroupBox0->isEnabled() ? ui->spinBox1->value() : 0;
    int value2 = ui->hGroupBox0->isEnabled() ? ui->spinBox2->value() : 0;

    //{"delay":0,"com":2,"brightness":1,"values":[0,0,0]}
    m_sendString = QString( "{\"delay\":%0,\"com\":%1,\"brightness\":%2,\"values\":[%3,%4,%5]}" ).arg( delay ).arg( com ).arg( brightness ).arg( value0 ).arg(
        value1 ).arg( value2 );
    m_timerSend.start( 200 );


}

uint FormPrograms::getButtonBackroundColorAsDecimal( QPushButton  *btnColor )
{
    QColor color = btnColor->palette().button().color();
    if( color.isValid() )
    {
        return HexToDec( color.name().right( 6 ) );
    }
    return 0;

}
void FormPrograms::SendFormColorsToSerial()
{
    if( m_timerSend.isActive() )
        m_timerSend.stop();

    uint color0 = getButtonBackroundColorAsDecimal( ui->btnColor0 );
    uint color1 = getButtonBackroundColorAsDecimal( ui->btnColor1 );
    uint color2 = getButtonBackroundColorAsDecimal( ui->btnColor2 );
    uint color3 = getButtonBackroundColorAsDecimal( ui->btnColor3 );
    uint color4 = getButtonBackroundColorAsDecimal( ui->btnColor4 );
    uint color5 = getButtonBackroundColorAsDecimal( ui->btnColor5 );

    //"{\"colors\":[16711935,16711680,32768,255,16777215,10824234]}"

    m_sendString = QString( "{\"colors\":[%0,%1,%2,%3,%4,%5]}" ).arg( color0 ).arg( color1 ).arg( color2 ).arg( color3 ).arg( color4 ).arg( color5 );
    m_timerSend.start( 200 );


}

void FormPrograms::on_pushButton_clicked()
{

    //SendFormValuesToSerial();
    //SendFormColorsToSerial();

    int delay = ui->spinBoxDelay->value();
    int com = ui->comboProgramName->currentIndex();
    int brightness = ui->spinBoxBrightness->value();
    int value0 = ui->hGroupBox0->isEnabled() ? ui->spinBox0->value() : 0;
    int value1 = ui->hGroupBox0->isEnabled() ? ui->spinBox1->value() : 0;
    int value2 = ui->hGroupBox0->isEnabled() ? ui->spinBox2->value() : 0;
    uint color0 = getButtonBackroundColorAsDecimal( ui->btnColor0 );
    uint color1 = getButtonBackroundColorAsDecimal( ui->btnColor1 );
    uint color2 = getButtonBackroundColorAsDecimal( ui->btnColor2 );
    uint color3 = getButtonBackroundColorAsDecimal( ui->btnColor3 );
    uint color4 = getButtonBackroundColorAsDecimal( ui->btnColor4 );
    uint color5 = getButtonBackroundColorAsDecimal( ui->btnColor5 );

    //"{\"delay\":6,\"com\":2,\"brightness\":11,\"values\":[0,0,0],\"colors\":[6754803,16711680,32768,255,16777215,10824234]}"
    QString str = QString( "{\"delay\":%0,\"com\":%1,\"brightness\":%2,\"values\":[%3,%4,%5],\"colors\":[%6,%7,%8,%9,%10,%11]}" )
                  .arg( delay ).arg( com ).arg( brightness )
                  .arg( value0 ).arg( value1 ).arg( value2 )
                  .arg( color0 ).arg( color1 ).arg( color2 ).arg( color3 ).arg( color4 ).arg( color5 );

    this->m_pApplication->m_Transport.send( str.toStdString().c_str() );

}

void FormPrograms::on_TimerSend()
{
    m_timerSend.stop();
    this->m_pApplication->m_Transport.send( m_sendString.toStdString().c_str() );
}

