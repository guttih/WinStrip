#include "mainwindow.h"

#include <QApplication>
#include <QFile>

int main( int argc, char *argv[] )
{
    QApplication a( argc, argv );
    MainWindow w;

    QFile styleSheetFile( "/home/gudjon/repos/personal/WinStrip/LinStrip/customstyles.css" );
    styleSheetFile.open( QFile::ReadOnly );
    QString styleSheet = QLatin1String( styleSheetFile.readAll() );


    a.setStyleSheet( styleSheet );
    w.show();
    return a.exec();
}
