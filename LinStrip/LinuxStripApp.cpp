#include "LinuxStripApp.h"

LinuxStripApp::LinuxStripApp( int &argc, char ** argv ) : QApplication( argc, argv )
{
    this->m_number = 1;
}

LinuxStripApp::~LinuxStripApp()
{

}

QSerialPort *LinuxStripApp::getSerialPort()
{
    return &m_serialPort;
}

SerialPortHandler *LinuxStripApp::getSerialHandler()
{
    return m_SerialHandler;
}

SerialPortHandler *LinuxStripApp::setSerialHandler( SerialPortHandler *pSerialPortHandler )
{
    this->m_SerialHandler = pSerialPortHandler;
    return this->m_SerialHandler;
}

LinuxStripApp *LinuxStripApp::instance()
{
    return ( LinuxStripApp* ) QApplication::instance();
}

