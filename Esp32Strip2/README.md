# Esp32Strip2

ESP32 firmware for a USB/Bluetooth-controlled LED strip, built with PlatformIO and the
Arduino framework. The Linux desktop controller is [LinStrip](../LinStrip/README.md).
Building the desktop application and flashing the ESP32 are separate steps.

See [development.md](development.md) for Rocky Linux 10 setup, build, upload,
and serial troubleshooting, [protocol.md](protocol.md) for the desktop/firmware
communication contract, and [test/README](test/README) for host regression tests.

The confirmed ESP32-WROOM-32 supports Bluetooth Classic SPP. The default build
advertises as **WinStrip** and serves the same protocol over Bluetooth and USB.
Replies go to the requesting connection; both control the same LED strip.

`esp32dev-usb-debug` reserves USB for startup/connection logs while commands use
Bluetooth. `esp32dev-usb-only` provides a build without the Bluetooth stack.
See [development.md](development.md) for build and hardware-check instructions.
LinStrip supports USB and Bluetooth and reads its effect list from the firmware.

Aurora (11) and Frost (12) are available in the firmware effect metadata.
See [effects.md](effects.md) for palettes, controls, and fast/slow sparkle examples.
