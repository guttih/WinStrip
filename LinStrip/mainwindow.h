#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QBluetoothDeviceDiscoveryAgent>
#include "FormCommands.h"
#include "FormPrograms.h"
#include "LinuxStripApp.h"

namespace Ui { class MainWindow; }
class MainWindow : public QMainWindow
{
    Q_OBJECT
public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow() override;
private slots:
    void on_action_About_2_triggered();
    void on_btnConnect_clicked();
    void refreshDevices();
private:
    void updateConnectionState(SerialTransport::State state);
    void showError(const QString &message);
    void handleReply(SERIAL_COMMAND command, const QString &reply);
    Ui::MainWindow *ui;
    FormCommands *m_formCommands;
    FormPrograms *m_formPrograms;
    LinuxStripApp *m_pApplication;
    QBluetoothDeviceDiscoveryAgent discovery;
};
#endif
