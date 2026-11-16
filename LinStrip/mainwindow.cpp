#include <QMessageBox>
#include <QSerialPortInfo>
#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    m_pApplication = LinuxStripApp::instance();
    m_formCommands = new FormCommands(this);
    m_formPrograms = new FormPrograms(this);
    // Replace the unused placeholder tab from the original form.
    QWidget *placeholder = ui->tabWidget->widget(0);
    ui->tabWidget->removeTab(0);
    delete placeholder;
    ui->tabWidget->addTab(m_formPrograms, tr("Programs"));
    ui->tabWidget->addTab(m_formCommands, tr("Manual"));
    auto &transport = m_pApplication->m_Transport;
    connect(&transport, &SerialTransport::stateChanged, this, &MainWindow::updateConnectionState);
    connect(&transport, &SerialTransport::errorOccurred, this, &MainWindow::showError);
    connect(&transport, &SerialTransport::queryReply, this, &MainWindow::handleReply);
    connect(&transport, &SerialTransport::dataHasCome, m_formCommands, &FormCommands::onNewData);
    connect(ui->btnRefresh, &QPushButton::clicked, this, &MainWindow::refreshDevices);
    connect(ui->comboTransport, &QComboBox::currentIndexChanged, this, [this] {
        discovery.stop();
        ui->comboDevices->clear();
        ui->comboDevices->setToolTip(ui->comboTransport->currentIndex() == 0
            ? tr("Select the ESP32 USB port.")
            : tr("Pair WinStrip in Linux Bluetooth settings, then discover and select it here."));
        refreshDevices();
    });
    connect(&discovery, &QBluetoothDeviceDiscoveryAgent::deviceDiscovered, this,
        [this](const QBluetoothDeviceInfo &device) {
            if (ui->comboTransport->currentIndex() != 1 ||
                m_pApplication->m_Transport.state() != SerialTransport::State::Disconnected ||
                device.address().isNull())
                return;
            const QString address = device.address().toString();
            if (ui->comboDevices->findData(address) < 0) {
                ui->comboDevices->addItem(tr("%1 (%2)").arg(device.name(), address), address);
                if (device.name() == "WinStrip")
                    ui->comboDevices->setCurrentIndex(ui->comboDevices->count() - 1);
            }
            ui->btnConnect->setEnabled(ui->comboDevices->count() > 0);
        });
    const auto discoveryFinished = [this] {
        ui->btnRefresh->setText(tr("Refresh / Discover"));
        if (ui->comboTransport->currentIndex() == 1 &&
            m_pApplication->m_Transport.state() == SerialTransport::State::Disconnected)
            ui->labelX->setText(ui->comboDevices->count() ? tr("Disconnected")
                : tr("Disconnected — no devices found. Check Bluetooth power and pairing, then refresh."));
    };
    connect(&discovery, &QBluetoothDeviceDiscoveryAgent::finished, this, discoveryFinished);
    connect(&discovery, &QBluetoothDeviceDiscoveryAgent::canceled, this, discoveryFinished);
    connect(&discovery, &QBluetoothDeviceDiscoveryAgent::errorOccurred, this,
        [this](QBluetoothDeviceDiscoveryAgent::Error) {
            ui->btnRefresh->setText(tr("Refresh / Discover"));
            if (ui->comboTransport->currentIndex() == 1 &&
                m_pApplication->m_Transport.state() == SerialTransport::State::Disconnected)
                showError(tr("Bluetooth discovery: %1").arg(discovery.errorString()));
        });
    updateConnectionState(transport.state());
    refreshDevices();
}

MainWindow::~MainWindow()
{
    disconnect(&discovery, nullptr, this, nullptr);
    discovery.stop();
    delete ui;
}

void MainWindow::on_action_About_2_triggered()
{
    QMessageBox::about(this, tr("About"), tr("LinStrip\nUSB and Bluetooth LED strip controller"));
}

void MainWindow::refreshDevices()
{
    if (m_pApplication->m_Transport.state() != SerialTransport::State::Disconnected)
        return;
    if (discovery.isActive()) {
        discovery.stop();
        if (ui->comboTransport->currentIndex() == 1)
            return;
    }
    const QString previous = ui->comboDevices->currentData().toString();
    ui->comboDevices->clear();
    if (ui->comboTransport->currentIndex() == 0) {
        ui->btnRefresh->setText(tr("Refresh / Discover"));
        for (const auto &port : QSerialPortInfo::availablePorts())
            ui->comboDevices->addItem(port.description().isEmpty() ? port.portName()
                : tr("%1 — %2").arg(port.portName(), port.description()), port.portName());
        const int index = ui->comboDevices->findData(previous);
        if (index >= 0)
            ui->comboDevices->setCurrentIndex(index);
        ui->labelX->setText(ui->comboDevices->count() ? tr("Disconnected") : tr("Disconnected — no USB ports found"));
    } else {
        ui->labelX->setText(tr("Disconnected — discovering Bluetooth devices…"));
        ui->btnRefresh->setText(tr("Stop discovery"));
        discovery.start(QBluetoothDeviceDiscoveryAgent::ClassicMethod);
    }
    ui->btnConnect->setEnabled(ui->comboDevices->count() > 0);
}

void MainWindow::on_btnConnect_clicked()
{
    auto &transport = m_pApplication->m_Transport;
    if (transport.state() != SerialTransport::State::Disconnected) {
        transport.disconnectDevice();
        return;
    }
    if (ui->comboDevices->currentIndex() < 0)
        return;
    discovery.stop();
    const QString target = ui->comboDevices->currentData().toString();
    if (ui->comboTransport->currentIndex() == 0)
        transport.connectToPort(target, 115200);
    else
        transport.connectToBluetooth(target);
}

void MainWindow::updateConnectionState(SerialTransport::State state)
{
    ui->statusbar->clearMessage();
    const bool disconnected = state == SerialTransport::State::Disconnected;
    const bool connected = state == SerialTransport::State::Connected;
    ui->comboTransport->setEnabled(disconnected);
    ui->comboDevices->setEnabled(disconnected);
    ui->btnRefresh->setEnabled(disconnected);
    ui->btnConnect->setEnabled(!disconnected || ui->comboDevices->count() > 0);
    ui->btnConnect->setText(disconnected ? tr("Connect") : connected ? tr("Disconnect") : tr("Cancel"));
    ui->labelX->setText(disconnected ? tr("Disconnected") : connected
        ? tr("Connected — %1").arg(ui->comboDevices->currentText())
        : tr("Connecting — %1").arg(ui->comboDevices->currentText()));
    m_formCommands->setEnabled(connected);
    // Enable effect controls only after fresh metadata from this connection.
    if (!connected)
        m_formPrograms->resetConnection();
}

void MainWindow::showError(const QString &message)
{
    ui->labelX->setText(tr("%1 — %2").arg(m_pApplication->m_Transport.isConnected()
        ? tr("Connected") : tr("Disconnected"), message));
    ui->statusbar->showMessage(message);
    m_formCommands->onNewData(tr("Error: %1").arg(message));
}

void MainWindow::handleReply(SERIAL_COMMAND command, const QString &reply)
{
    if (command == ALLSTATUS && !m_formPrograms->AllStatusToForm(reply))
        showError(tr("Invalid ALLSTATUS metadata. Retry ALLSTATUS from Manual."));
    // PROGRAMINFO remains available in Manual; only ALLSTATUS has both metadata
    // and the current values required to safely initialize program controls.
}
