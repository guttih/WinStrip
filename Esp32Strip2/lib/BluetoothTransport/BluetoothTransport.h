#ifndef BLUETOOTH_TRANSPORT_H
#define BLUETOOTH_TRANSPORT_H

#include <BluetoothSerial.h>
#include "BluetoothSession.h"

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED) || !defined(CONFIG_BT_SPP_ENABLED)
#error This firmware requires an ESP32 with Bluetooth Classic SPP support.
#endif

class BluetoothTransport : public Stream
{
public:
    bool begin( const char *name );
    // Bind loop reads/replies to one session. Returns true after any transition,
    // including a disconnect and reconnect occurring between loop iterations.
    bool selectSession();
    bool isCurrentSession();
    void poll();

    int available() override;
    int read() override;
    int peek() override;
    size_t write( uint8_t value ) override;
    size_t write( const uint8_t *buffer, size_t length ) override;
    void flush() override {} // Replies are sent asynchronously by poll().

private:
    static BluetoothTransport *instance;
    static void onEvent( esp_spp_cb_event_t event, esp_spp_cb_param_t *param );

    BluetoothSerial radio;
    BluetoothSession session;
    portMUX_TYPE mutex = portMUX_INITIALIZER_UNLOCKED;
    uint32_t selectedGeneration = 0;
    uint8_t packet[ BluetoothSession::PACKET_SIZE ] = {};
    bool started = false;
};

#endif
