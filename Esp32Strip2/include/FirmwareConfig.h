#ifndef FIRMWARE_CONFIG_H
#define FIRMWARE_CONFIG_H

#ifndef STRIP_ENABLE_BLUETOOTH
#define STRIP_ENABLE_BLUETOOTH 1
#endif

#ifndef STRIP_USB_DEBUG
#define STRIP_USB_DEBUG 0
#endif

#if STRIP_USB_DEBUG && !STRIP_ENABLE_BLUETOOTH
#error USB debug mode requires Bluetooth for controller commands.
#endif

#if STRIP_USB_DEBUG
#define STRIP_LOG( ... ) Serial.printf( __VA_ARGS__ )
#else
#define STRIP_LOG( ... ) ( (void) 0 )
#endif

#endif
