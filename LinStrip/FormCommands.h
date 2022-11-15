#ifndef FORMCOMMANDS_H
#define FORMCOMMANDS_H

#include <QWidget>
#include "serialcommands.h"
#include "SerialPortHandler.h"
#include "LinuxStripApp.h"

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
    QTextEdit *GetTextEditResponce();

private slots:
    void showEvent( QShowEvent* event );
    void hideEvent( QHideEvent* event );

    void on_btnSendCommand_clicked();

    void on_pushButton_clicked();

private:
    Ui::FormCommands *ui;
    LinuxStripApp *m_pApplication = nullptr;

};

#endif // FORMCOMMANDS_H
