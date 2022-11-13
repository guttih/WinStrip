#ifndef LINUXSTRIPAPP_H
#define LINUXSTRIPAPP_H

#include "SerialPortHandler.h"
#include <QApplication>
#include "SerialTransport.h"

struct AppInfo {
    QString workingDirectory;
    QString fileName;
};

class LinuxStripApp : public QApplication
{
public:
    LinuxStripApp( int &argc, char ** argv );
    ~LinuxStripApp();
    static LinuxStripApp *instance();
    SerialTransport m_Transport;

private:

};

#endif
