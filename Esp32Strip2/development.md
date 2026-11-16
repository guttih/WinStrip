# ESP32 firmware development on Rocky Linux 10

## Development environment

Open this directory (`Esp32Strip2`, containing `platformio.ini`) in VS Code
and install the recommended **PlatformIO IDE** extension
(`platformio.platformio-ide`). It supplies PlatformIO Core and manages the
ESP32 compiler, Arduino framework, and FastLED dependency. A separate Arduino
IDE or system ESP32 compiler is not required.

Official instructions:
[PlatformIO IDE](https://docs.platformio.org/en/stable/integration/ide/vscode.html)
and [Linux USB rules](https://docs.platformio.org/en/stable/core/installation/udev-rules.html).

Use a PlatformIO terminal for the commands below. If `pio` is unavailable in a
normal terminal, use `~/.platformio/penv/bin/pio` instead.

## Build

From this directory:

```bash
pio run -e esp32dev
```

The checkmark **Build** button in PlatformIO performs the same task.
The first build downloads dependencies and needs internet access.

Hardware settings live near the beginning of `src/main.cpp`:

| Setting | Current value |
| --- | --- |
| Board | Generic ESP32 development module (`esp32dev`) |
| Strip | APA102 |
| LED count | 62 |
| Data pin | GPIO 14 |
| Clock pin | GPIO 13 |
| Color order | BGR |
| Serial baud rate | 115200 |

Check these against your actual hardware before uploading. The firmware starts
a rainbow animation at brightness 90. Use the strip's intended power supply
and a common ground with the ESP32.

## USB access

Connect the ESP32 with a data-capable USB cable and inspect its port:

```bash
pio device list
ls -l /dev/ttyUSB* /dev/ttyACM*
id -nG
```

An unmatched device pattern is harmless if the other lists your board.
If neither lists a device, try another cable or USB port.

If the port belongs to `dialout`, ensure your user belongs to that group:

```bash
sudo usermod -aG dialout "$USER"
```

Log out of the desktop completely and log back in, then check `id -nG` again.
Membership in `tty` is not normally needed for a `dialout`-owned USB serial port.
For missing PlatformIO USB rules, follow the official Linux USB rules link above.
Run PlatformIO and VS Code as your normal user.

## Upload and check communication

Close LinStrip or any other application using the port first. Replace
`/dev/ttyUSB0` with the actual device:

```bash
pio run -e esp32dev -t upload --upload-port /dev/ttyUSB0
pio device monitor --port /dev/ttyUSB0 --baud 115200 --eol LF
```

Upload replaces the firmware currently on the board. If uploading stalls at
`Connecting`, try holding the board's BOOT button while it connects, then
release it once writing starts.

In the monitor, send `STATUS` followed by Enter; the firmware should reply `OK`.
`HARDWARE`, `PIXELCOUNT`, and `ALLSTATUS` provide more information. Commands are
case-sensitive and must end with LF; CRLF is also accepted. Replies use CRLF.
Exit the monitor with Ctrl+C before opening LinStrip or uploading again.
See [protocol.md](protocol.md) for chunking, buffer limits, and response formats.

For the desktop controller, follow [LinStrip's build/run guide](../LinStrip/README.md).
It uses Qt 6 and USB serial at 115200 baud.

## Debugging and the Bluetooth preparation

The application no longer writes startup banners or effect/JSON debug text to
USB. USB remains a protocol connection, so a serial monitor can send queries
and inspect replies whenever LinStrip has released the port. Opening a monitor
may reset the board.

USB and Bluetooth are separate connections. Once Bluetooth is implemented and
LinStrip uses it, USB can carry development diagnostics while Bluetooth carries
commands and replies. This needs an explicit logging option that keeps USB
logging disabled when a desktop controller uses USB. The current firmware has
neither Bluetooth initialization nor a USB debug-logging option.

The transport preparation provides a separate bounded `SerialReader(Stream&)`
for each future connection. Reset its buffer on disconnect. The Stars effect
still contains blocking delays that must be removed before enabling Bluetooth.
Confirm the module supports Bluetooth Classic before adding `BluetoothSerial`;
the configured `esp32dev` board and the USB bridge do not identify the chip.

Run the host regression tests without a connected board:

```bash
bash test/run_host_tests.sh
```

See [test/README](test/README) for coverage and optional sanitizers.

## Verification on the development machine

Inspected on 2026-11-10, running Rocky Linux 10.2:

- VS Code, PlatformIO IDE, and PlatformIO Core 6.2.0 are installed.
- Installed dependencies resolve to Espressif32 7.0.1, Arduino-ESP32 2.0.17,
  and FastLED 3.10.9. `platformio.ini` currently permits dependency updates.
- FastLED compatibility required replacing three `struct CRGB` declarations
  with `CRGB`, because the installed FastLED exposes it as a type alias, and
  explicitly including `Arduino.h` for Arduino types and functions.
- `pio run` succeeded after these fixes (350,729 bytes flash, 23,212 bytes RAM).
- `/etc/udev/rules.d/99-platformio-udev.rules` is present.
- The account is listed in `dialout`, but the inspected process's active groups
  do not include it: start a fresh login session before accessing the board.
- No `/dev/ttyUSB*` or `/dev/ttyACM*` device was visible. Upload and physical LED
  operation still require verification with the board connected.

On 2026-11-16, after the transport preparation:

- `pio run -e esp32dev` succeeded: 348,773 bytes flash of 1,310,720 and
  26,500 bytes static RAM of 327,680. The fixed receive buffer now appears in
  static RAM usage rather than an Arduino `String` allocation on the heap.
- All 15 native reader/reply tests passed. The installed host compiler could
  not link AddressSanitizer because its runtime is missing; the successful run
  used the default tests without sanitizers.
- The user reported an earlier successful USB build/upload. This refactor was
  built and host-tested only; it was not uploaded or checked against hardware,
  desktop controls, or physical LEDs. Bluetooth is still disabled.
