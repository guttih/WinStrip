#ifndef FORMCOMMANDS_H
#define FORMCOMMANDS_H

#include <QWidget>
#include "./serialcommands.h"

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

private:
    Ui::FormCommands *ui;
};

#endif // FORMCOMMANDS_H
