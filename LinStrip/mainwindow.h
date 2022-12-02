#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "SerialPortHandler.h"
#include "FormCommands.h"
#include "FormPrograms.h"
#include "LinuxStripApp.h"

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
    void on_tabWidget_tabBarClicked( int index );
    void on_btnConnect_clicked();
    void on_newData( QString str );

    void on_tabWidget_currentChanged( int index );

private:
    Ui::MainWindow *ui;
    bool processCommandResponse( QString response );
    bool connectToPort( const QString &name, int baudRate );
    FormCommands *m_formCommmands = nullptr;
    FormPrograms *m_formPrograms = nullptr;
    LinuxStripApp *m_pApplication = nullptr;
};
#endif // MAINWINDOW_H
