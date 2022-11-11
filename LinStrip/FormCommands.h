#ifndef FORMCOMMANDS_H
#define FORMCOMMANDS_H

#include <QWidget>
#include "./serialcommands.h"
#include "./SerialPortHandler.h"



namespace Ui
{
class FormCommands;
}

class FormCommands : public QWidget
{
Q_OBJECT

public:
    explicit FormCommands( QWidget *parent = nullptr );
    ~FormCommands();
    SerialPortHandler *m_SerialPortHandler = nullptr;
    QTextEdit *GetTextEditResponce();
    void setMainForm( QWidget *mainWindow );

private slots:
    void showEvent( QShowEvent* event );
    void hideEvent( QHideEvent* event );

    void on_btnSendCommand_clicked();

    void on_pushButton_clicked();

private:
    Ui::FormCommands *ui;
    QWidget  *m_mainWindow = nullptr;
};

#endif // FORMCOMMANDS_H
