#include <Arduino.h>
#include <esp_log.h>
#include "FirmwareConfig.h"
#include "SerialReader.h"
#include "Json.h"

#if STRIP_ENABLE_BLUETOOTH
#include "BluetoothTransport.h"
BluetoothTransport bluetooth;
SerialReader bluetoothConnection( bluetooth );
const char *bluetoothName = "WinStrip";
#endif
/*
 Name:		Esp32Strip.ino
 Created:	3/14/2020 11:24:27 AM
 Author:	gutti
 If you need to send longer strings than 255 split them up by adding "@" at then end of the string

     For example, if you want to send the string "This is a long string.\n" in three shipments,
     you could send it like this:
        1) "This is@\n"
        2) " a long@\n"
        3) " string.\n"
    See SerialReader::printLine for reply framing.
*/

#if !STRIP_USB_DEBUG
SerialReader usbConnection( Serial );
#endif

#include "StripHelper.h"

//#define NUM_LEDS 116
#define NUM_LEDS 62
#define CLOCK_PIN 13  /*green wire*/
#define DATA_PIN  14  /*blue wire*/

#define STRIP_TYPE APA102
const char* stripType = "APA102";
#define COLOR_SCHEME BGR

/*
// #define STRIP_TYPE WS2801
#define STRIP_TYPE WS2811
#define COLOR_SCHEME RBG
const char* stripType = "WS2811";
*/


CRGB leds[ NUM_LEDS ];
StripHelper stripper;


struct COMMAND_TEXT
{
    const char Name[ 20 ];
};

enum COMMAND
{
    COMMAND_INVALID,
    COMMAND_STATUS,
    COMMAND_BUFFERSIZE,
    COMMAND_SEPARATOR,
    COMMAND_PROGRAMCOUNT,
    COMMAND_PROGRAMINFO,
    COMMAND_ALLSTATUS,
    COMMAND_COLORS,
    COMMAND_VALUES,
    COMMAND_PIXELCOUNT,
    COMMAND_HARDWARE,
    COMMAND_VALUES_COLORS,
    COMMAND_COUNT
};

COMMAND_TEXT commands[ COMMAND::COMMAND_COUNT ] = {
    "INVALID",
    "STATUS",
    "BUFFERSIZE",
    "SEPARATOR",
    "PROGRAMCOUNT",
    "PROGRAMINFO",
    "ALLSTATUS",
    "COLORS",
    "VALUES",
    "PIXELCOUNT",
    "HARDWARE",
    "VALUES_COLORS"
};


void runCommand( int cmd, SerialReader &connection )
{
    switch( cmd )
    {
        case COMMAND_STATUS:
            connection.printLine( "OK" );
            break;
        case COMMAND_BUFFERSIZE:
            connection.printLine( String( connection.getMaxLength() ).c_str() );
            break;
        case COMMAND_SEPARATOR:
            connection.printLine( String( connection.getSeparator() ).c_str() );
            break;
        case COMMAND_PROGRAMCOUNT:
            connection.printLine( String( stripper.getProgramCount() ).c_str() );
            break;
        case COMMAND_PROGRAMINFO:
            connection.printLine( stripper.getAllProgramInfosAsJsonArray().c_str() );
            break;
        case COMMAND_ALLSTATUS:
            connection.printLine( stripper.toJson().c_str() );
            break;
        case COMMAND_COLORS:
            connection.printLine( ( "{" + stripper.MakeJsonKeyVal( "colors", stripper.getColorsAsJson() ) + "}" ).c_str() );
            break;
        case COMMAND_VALUES:
            connection.printLine( stripper.getValuesAsJson().c_str() );
            break;
        case COMMAND_HARDWARE:
            connection.printLine( stripper.getHardwareAsJson().c_str() );
            break;
        case COMMAND_PIXELCOUNT:
            connection.printLine( String( stripper.getCount() ).c_str() );
            break;
        case COMMAND_VALUES_COLORS:
            connection.printLine( stripper.getValuesAndColorsAsJson().c_str() );
            break;

    }
}

// Return COMMAND_INVALID if no command is found.
int getCommand( const char *str )
{
    for( int i = COMMAND::COMMAND_STATUS; i < COMMAND::COMMAND_COUNT; i++ )
    {
        if( strcmp( str, commands[ i ].Name ) == 0 )
            return i;
    }
    return COMMAND::COMMAND_INVALID;
}


void processColors( JsonData *root )
{
    JsonData *current;
    unsigned long ulColor;
    int i;
    current = root->getChild( "colors" );
    if( current != NULL && current->getValueType() == JSONTYPE::JSONTYPE_ARRAY )
    {
        //Colors are optional
        //we got the key, let's get the array
        current = current->getChildAt( 0 );
        //now let's get the first item in the array
        if( current ) current = current->getChildAt( 0 );
        i = 0;
        while( current && i < COLOR_COUNT )
        {
            ulColor = current->getValueAsULong();
            stripper.setColorBank( i, stripper.decodeColor( ulColor ) );
            current = current->getNext();
            i++;
        }
    }


}

void processJson( const char *str )
{
    Json parser( str );
    if( !parser.isValid() )
    {
        STRIP_LOG( "[protocol] Invalid JSON\r\n" );
        return;
    }


    JsonData *root = parser.getRootObject();
    if( root == NULL || root->getType() != JSONTYPE_OBJECT )
        return;



    JsonData* current;
    unsigned long ulCom, ulDelay, ulBrightness;
    const int VARIABLE_COUNT = 3;
    unsigned long values[ VARIABLE_COUNT ] = { 0, 0, 0 };
    int i;


    current = root->getChild( "com" );
    if( current == NULL )
    {
        processColors( root );
        return;
    }
    ulCom = current->getValueAsULong();
    if( ulCom >= STRIP_PROGRAMS_COUNT ) return;

    current = root->getChild( "delay" );
    if( current == NULL )
    {
        return;
    }
    ulDelay = current->getValueAsULong();

    current = root->getChild( "brightness" );
    if( current == NULL )
    {
        return;
    }
    ulBrightness = current->getValueAsULong();


    current = root->getChild( "values" );
    if( current == NULL )
        current = root->getChild( "values" );
    if( current == NULL || current->getValueType() != JSONTYPE::JSONTYPE_ARRAY )
    {
        return;
    }
    current = current->getChildAt( 0 );//we got the key, let's get the array
    if( current ) current = current->getChildAt( 0 );//first array item

    i = 0;
    while( current && i < VARIABLE_COUNT )
    {
        values[ i ] = current->getValueAsULong();
        current = current->getNext();
        i++;
    }

    stripper.setBrightness( ulBrightness );
    stripper.setNewValues( ( STRIP_PROGRAMS ) ulCom, ulDelay, values[ 0 ], values[ 1 ], values[ 2 ] );
    processColors( root ); // Apply explicit color-bank updates, when supplied.
    stripper.initProgram( stripper.getProgram() );

}

void processNewString( SerialReader &connection )
{
    const char *message = connection.getMessage();
    size_t len = strlen( message );

    if( len < 1 )
    {
        return; //nothing to do
    }

    char firstChar = message[ 0 ];

    if( firstChar == '{' )
    {
        processJson( message );
    }
    else if( len < 20 )
    {
        int i = getCommand( message );
        if( i != COMMAND_INVALID )
            runCommand( i, connection );
    }

}

void serviceConnection( SerialReader &connection, bool ( *isCurrent )() = nullptr )
{
    if( connection.readMessage() == SerialReader::ReadResult::MessageReady )
    {
        if( isCurrent == nullptr || isCurrent() )
            processNewString( connection );
        connection.resetBuffer();
    }
}

#if STRIP_ENABLE_BLUETOOTH
bool bluetoothSessionIsCurrent()
{
    return bluetooth.isCurrentSession();
}

void serviceBluetooth()
{
    bluetooth.poll();
    if( bluetooth.selectSession() )
    {
        bluetoothConnection.resetBuffer();
        STRIP_LOG( "[Bluetooth] %s; free heap %u\r\n",
            bluetooth.isCurrentSession() ? "connected" : "disconnected",
            ESP.getFreeHeap() );
    }
    if( bluetooth.isCurrentSession() )
        serviceConnection( bluetoothConnection, bluetoothSessionIsCurrent );
}
#endif

void stripInit()
{
    pinMode( DATA_PIN, OUTPUT );
    pinMode( CLOCK_PIN, OUTPUT );
    FastLED.addLeds< STRIP_TYPE, DATA_PIN, CLOCK_PIN, COLOR_SCHEME >( leds, NUM_LEDS );
    //FastLED.addLeds< STRIP_TYPE, DATA_PIN, RGB >( leds,  NUM_LEDS );
    //FastLED.addLeds<WS2811,     DATA_PIN, RGB>(leds, NUM_LEDS);
    stripper.initialize( &FastLED, stripType, COLOR_SCHEME, DATA_PIN, CLOCK_PIN );
    stripper.initProgram( stripper.getProgram() );
}

void setup()
{
    // initialize serial:
    Serial.begin( 115200 );
    // Keep framework/driver diagnostics out of the USB protocol too.
    esp_log_level_set( "*", ESP_LOG_NONE );

    stripInit();


    //todo: remove after debugging
    stripper.setBrightness( 90 );
    // stripper.setNewValues( STRIP_PROGRAMS::RAINBOW, 30, 1, 0, 0 );
    stripper.setNewValues( STRIP_PROGRAMS::RAINBOW, 30, 1, 15, 0 );
    stripper.initProgram( stripper.getProgram() );

#if STRIP_ENABLE_BLUETOOTH
    bool ready = bluetooth.begin( bluetoothName );
    STRIP_LOG( "[Bluetooth] %s: %s; free heap %u\r\n", bluetoothName,
        ready ? "started" : "initialization failed", ESP.getFreeHeap() );
    (void) ready;
#endif
}

void loop()
{
#if !STRIP_USB_DEBUG
    serviceConnection( usbConnection );
#endif
#if STRIP_ENABLE_BLUETOOTH
    serviceBluetooth();
#endif

    stripper.run();

}
