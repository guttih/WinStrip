#ifndef FORMPROGRAMS_H
#define FORMPROGRAMS_H

#include <QWidget>
#include <QPushButton>
#include "LinuxStripApp.h"
#include "SerialProgramInformation.h"

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


private:
    Ui::FormPrograms *ui;
    void btnColor_clicked( QPushButton *btnColor );
    QList< SerialProgramInformation * > m_ProgramList;
    LinuxStripApp *m_pApplication = nullptr;
};

#endif // FORMPROGRAMS_H
