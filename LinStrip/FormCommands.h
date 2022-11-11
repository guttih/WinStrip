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

private slots:
    void on_FormCommands_windowTitleChanged( const QString &title );
    void showEvent( QShowEvent* event );
    void hideEvent( QHideEvent* event );

private:
    Ui::FormCommands *ui;
};

#endif // FORMCOMMANDS_H
