#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "./SerialPortHandler.h"
#include "./FormCommands.h"

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
Q_OBJECT

public:
    MainWindow( QWidget *parent = nullptr );
    ~MainWindow();
    SerialPortHandler *getSerialHandler()
    {
        return this->m_SerialHandler;
    }


private slots:
    void on_action_About_2_triggered();

    void on_MainWindow_iconSizeChanged( const QSize &iconSize );

    void on_tabWidget_tabBarClicked( int index );

    void on_btnConnect_clicked();

private:
    Ui::MainWindow *ui;
    QSerialPort m_serialPort;

    bool connectToPort( const QString &name, int baudRate );
    FormCommands *m_formCommmands = nullptr;
    SerialPortHandler *m_SerialHandler = nullptr;
};
#endif // MAINWINDOW_H
