#include "mainwindow.h"
#include "./ui_mainwindow.h"
#include <QMessageBox>
#include "./formcommands.h"
#include <QFile>
MainWindow::MainWindow( QWidget *parent )
    : QMainWindow( parent )
    , ui( new Ui::MainWindow )
{
    ui->setupUi( this );
    ui->tabWidget->addTab( new FormCommands(), QString( "Manual" ).arg( ui->tabWidget->count() + 1 ) );
}

MainWindow::~MainWindow()
{
    delete ui;
}


void MainWindow::on_action_About_2_triggered()
{
    QMessageBox::about( this, "About", "\nLinStrip\n    Version 1.0     \n" );
}

