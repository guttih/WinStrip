#ifndef FORMPROGRAMS_H
#define FORMPROGRAMS_H

#include <QWidget>

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

private:
    Ui::FormPrograms *ui;
};

#endif // FORMPROGRAMS_H
