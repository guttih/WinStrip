#include "BluetoothTransport.h"

BluetoothTransport *BluetoothTransport::instance = nullptr;

bool BluetoothTransport::begin( const char *name )
{
    instance = this; // BluetoothSerial supports one server instance.
    radio.register_callback( onEvent );
    // Bypass BluetoothSerial's 512-byte RX queue. The event handler below
    // stores bytes by handle in our bounded session buffer instead.
    radio.onData( []( const uint8_t *, size_t ) {} );
    radio.setTimeout( 0 );
    started = radio.begin( name );
    return started;
}

void BluetoothTransport::onEvent( esp_spp_cb_event_t event, esp_spp_cb_param_t *param )
{
    if( instance == nullptr )
        return;
    BluetoothTransport &transport = *instance;
    uint32_t now = millis();
    portENTER_CRITICAL( &transport.mutex );
    switch( event )
    {
        case ESP_SPP_SRV_OPEN_EVT:
            if( param->srv_open.status == ESP_SPP_SUCCESS )
                transport.session.open( param->srv_open.handle );
            break;
        case ESP_SPP_CLOSE_EVT:
            if( param->close.async || param->close.status == ESP_SPP_SUCCESS )
                transport.session.close( param->close.handle );
            break;
        case ESP_SPP_DATA_IND_EVT:
            transport.session.receive( param->data_ind.handle, param->data_ind.data,
                                       param->data_ind.len );
            break;
        case ESP_SPP_CONG_EVT:
            transport.session.setCongested( param->cong.handle, param->cong.cong, now );
            break;
        case ESP_SPP_WRITE_EVT:
            transport.session.writeComplete( param->write.handle,
                param->write.status == ESP_SPP_SUCCESS, param->write.cong, now );
            break;
        default:
            break;
    }
    portEXIT_CRITICAL( &transport.mutex );
}

bool BluetoothTransport::selectSession()
{
    portENTER_CRITICAL( &mutex );
    uint32_t generation = session.getGeneration();
    portEXIT_CRITICAL( &mutex );
    bool changed = generation != selectedGeneration;
    selectedGeneration = generation;
    return changed;
}

bool BluetoothTransport::isCurrentSession()
{
    portENTER_CRITICAL( &mutex );
    bool connected = session.isCurrent( selectedGeneration );
    portEXIT_CRITICAL( &mutex );
    return connected;
}

void BluetoothTransport::poll()
{
    if( !started )
        return;
    uint32_t now = millis();
    uint32_t handle = 0, generation = 0;
    portENTER_CRITICAL( &mutex );
    size_t length = session.nextPacket( now, packet, handle, generation );
    uint32_t disconnectHandle = session.takeDisconnectRequest();
    portEXIT_CRITICAL( &mutex );

    // Use the captured SPP handle rather than BluetoothSerial::write, whose
    // shared TX queue can block and survive a client change. Never wait for
    // congestion or write completion in the Arduino loop.
    if( length > 0 && esp_spp_write( handle, length, packet ) != ESP_OK )
    {
        portENTER_CRITICAL( &mutex );
        session.fail( handle, generation );
        portEXIT_CRITICAL( &mutex );
    }
    if( disconnectHandle != 0 )
        esp_spp_disconnect( disconnectHandle );
}

int BluetoothTransport::available()
{
    portENTER_CRITICAL( &mutex );
    int count = session.available( selectedGeneration );
    portEXIT_CRITICAL( &mutex );
    return count;
}

int BluetoothTransport::read()
{
    portENTER_CRITICAL( &mutex );
    int value = session.read( selectedGeneration );
    portEXIT_CRITICAL( &mutex );
    return value;
}

int BluetoothTransport::peek()
{
    portENTER_CRITICAL( &mutex );
    int value = session.peek( selectedGeneration );
    portEXIT_CRITICAL( &mutex );
    return value;
}

size_t BluetoothTransport::write( uint8_t value )
{
    return write( &value, 1 );
}

size_t BluetoothTransport::write( const uint8_t *buffer, size_t length )
{
    portENTER_CRITICAL( &mutex );
    size_t written = session.write( selectedGeneration, buffer, length );
    portEXIT_CRITICAL( &mutex );
    return written;
}
