#include "LinuxStripApp.h"
#include "SerialTransport.cpp"

LinuxStripApp::LinuxStripApp( int &argc, char ** argv ) : QApplication( argc, argv )
{
}

LinuxStripApp::~LinuxStripApp()
{

}

LinuxStripApp *LinuxStripApp::instance()
{
    return ( LinuxStripApp* ) QApplication::instance();
}

