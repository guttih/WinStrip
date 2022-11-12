#ifndef LINUXSTRIPAPP_H
#define LINUXSTRIPAPP_H

#include "SerialPortHandler.h"
#include <QApplication>
#include <QtSerialPort/QSerialPort>

struct AppInfo {
    QString workingDirectory;
    QString fileName;
};

class LinuxStripApp : public QApplication
{
public:
    static LinuxStripApp *instance();
    QSerialPort *getSerialPort();
    SerialPortHandler *getSerialHandler();
    SerialPortHandler *setSerialHandler( SerialPortHandler *pSerialPortHandler );

    LinuxStripApp( int &argc, char ** argv );
    ~LinuxStripApp();

private:
    int m_number = 0;
    QSerialPort m_serialPort;
    SerialPortHandler *m_SerialHandler = nullptr;

};

#endif
