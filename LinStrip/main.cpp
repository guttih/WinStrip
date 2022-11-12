#include "mainwindow.h"
#include "LinuxStripApp.h"
// #include <QApplication>
#include <QFile>
#include <QDebug>

int main( int argc, char *argv[] )
{
    LinuxStripApp a( argc, argv );
    MainWindow w;

    QFile styleSheetFile( "/home/gudjon/repos/personal/WinStrip/LinStrip/customstyles.css" );
    styleSheetFile.open( QFile::ReadOnly );
    QString styleSheet = QLatin1String( styleSheetFile.readAll() );


    a.setStyleSheet( styleSheet );
    w.show();
    return a.exec();
}
