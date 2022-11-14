#ifndef LINUXSTRIPAPP_H
#define LINUXSTRIPAPP_H

#include "SerialPortHandler.h"
#include <QApplication>
#include "SerialTransport.h"
#include <QTimer>

struct AppInfo {
    QString workingDirectory;
    QString fileName;
    QString inputFileName;
    bool pullExternalCommands = true;
};

class LinuxStripApp : public QApplication
{
public:
    LinuxStripApp( int &argc, char ** argv );
    ~LinuxStripApp();
    static LinuxStripApp *instance();
    SerialTransport m_Transport;
    void CheckAndGetExternalCommands();

private:

    QTimer m_ReadFileTimer;
    AppInfo m_ApplicationInformation;
};

#endif
