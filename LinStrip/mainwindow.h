#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "./SerialPortHandler.h"

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
Q_OBJECT

public:
    MainWindow( QWidget *parent = nullptr );
    ~MainWindow();

private slots:
    void on_action_About_2_triggered();

    void on_MainWindow_iconSizeChanged( const QSize &iconSize );

    void on_tabWidget_currentChanged( int index );

    void on_tabWidget_tabBarClicked( int index );

private:
    Ui::MainWindow *ui;
    SerialPortHandler *m_SerialHandler = nullptr;
};
#endif // MAINWINDOW_H
