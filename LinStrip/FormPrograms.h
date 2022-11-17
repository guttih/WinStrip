#ifndef FORMPROGRAMS_H
#define FORMPROGRAMS_H

#include <QWidget>
#include <QPushButton>
#include "LinuxStripApp.h"
#include "SerialProgramInformation.h"
#include "SerialStatus.h"

namespace Ui
{
class FormPrograms;
}

class FormPrograms : public QWidget
{
Q_OBJECT

public:
    explicit FormPrograms( QWidget *parent = nullptr );
    ~FormPrograms();
    void showEvent( QShowEvent* event );
    bool ProgramsToForm( QString programJsonList );
    bool AllStatusToForm( QString jsonAllStatus );

private slots:
    void on_btnColor0_clicked();
    void on_btnColor1_clicked();
    void on_btnColor2_clicked();
    void on_btnColor3_clicked();
    void on_btnColor4_clicked();
    void on_btnColor5_clicked();

    uint HexToDec( QString hexString );
    QString DecToHex( int number );

    void on_commandLinkButton_clicked();
    void on_hSlider0_valueChanged( int value );
    void on_hSlider1_valueChanged( int value );
    void on_hSlider2_valueChanged( int value );
    void on_sliderDelay_valueChanged( int value );
    void on_sliderBrightness_valueChanged( int value );
    void on_spinBox0_valueChanged( int value );
    void on_spinBox1_valueChanged( int value );
    void on_spinBox2_valueChanged( int value );
    void on_spinBoxDelay_valueChanged( int value );
    void on_spinBoxBrightness_valueChanged( int value );
    void on_comboProgramName_currentIndexChanged( int index );
    void on_pushButton_clicked();
    void on_TimerSend();

private:
    Ui::FormPrograms *ui;
    void btnColor_clicked( QPushButton *btnColor );
    QList< SerialProgramInformation * > m_ProgramList;
    SerialStatus m_SerialStatus;
    LinuxStripApp *m_pApplication = nullptr;
    void SerialStatusToForm();
    void ColorToFormButton( QPushButton *btnColor, QString strHexidecimalColor );
    void SendFormValuesToSerial();
    void SendFormColorsToSerial();
    uint getButtonBackroundColorAsDecimal( QPushButton *btnColor );
    QTimer m_timerSend;
    QString m_sendString;
};

#endif // FORMPROGRAMS_H
