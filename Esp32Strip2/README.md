# Esp32Strip2

ESP32 firmware for a USB-controlled LED strip, built with PlatformIO and the
Arduino framework. The Linux desktop controller is [LinStrip](../LinStrip/README.md).
Building the desktop application and flashing the ESP32 are separate steps.

See [development.md](development.md) for Rocky Linux 10 setup, build, upload,
and serial troubleshooting, [protocol.md](protocol.md) for the desktop/firmware
communication contract, and [test/README](test/README) for host regression tests.

The message reader and replies accept a separate Arduino `Stream` per connection
in preparation for Bluetooth. The current firmware still enables USB only.
