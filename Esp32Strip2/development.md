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

The physical module was identified by the user as **ESP32-WROOM-32** on
2026-11-16. This supports Bluetooth Classic SPP. The standard `esp32dev` build
now enables Bluetooth alongside the existing USB protocol. Dependencies and
the default flash partition layout are unchanged.
See the [ESP32-WROOM-32 datasheet](https://documentation.espressif.com/esp32-wroom-32_datasheet_en.html)
for the module's Bluetooth BR/EDR support.

| PlatformIO environment | USB | Bluetooth |
| --- | --- | --- |
| `esp32dev` (default) | Protocol, 115200 baud | SPP, name `WinStrip` |
| `esp32dev-usb-debug` | Development logs, 115200 baud | SPP commands/replies |
| `esp32dev-usb-only` | Protocol, 115200 baud | Disabled |

Build the logging or recovery variants with `pio run -e esp32dev-usb-debug` or
`pio run -e esp32dev-usb-only`. A bare `pio run` builds only `esp32dev`.

Hardware settings live near the beginning of `src/main.cpp`:

| Setting | Current value |
| --- | --- |
| Board/module | `esp32dev`, user-confirmed ESP32-WROOM-32 |
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

## Bluetooth and USB debugging

After uploading `esp32dev`, pair with **WinStrip** in the Linux Bluetooth
settings and open its Serial Port Profile (SPP/RFCOMM) service in a compatible
client. The firmware accepts one Bluetooth client at a time. Pairing follows the
installed Arduino-ESP32 2.0.17 defaults; a legacy PIN prompt uses `1234`. A paired
device is not necessarily connected to SPP. LinStrip currently supports USB
only; its Qt Bluetooth client still needs implementation.

The normal build keeps application/driver diagnostics out of both protocol
connections. A USB serial monitor can send queries whenever LinStrip has
released the port. For startup/connection logs while using Bluetooth commands,
build and upload the explicit debug environment, replacing the example port:

```bash
pio run -e esp32dev-usb-debug -t upload --upload-port /dev/ttyUSB0
pio device monitor --port /dev/ttyUSB0 --baud 115200 --eol LF
```

In this environment USB carries logs only; sending `STATUS` over USB will not
produce a protocol reply. Logs begin in setup, before any Bluetooth client
connects, and include initialization, connection transitions, and current free
heap. Invalid JSON is also logged without echoing its contents. Opening a USB
monitor can reset the board and disconnect Bluetooth. Use the normal build
again for USB controller compatibility.

Each connection has its own `SerialReader(Stream&)`. Bluetooth callbacks copy
bytes and update session state under a lock; the Arduino loop runs all commands
and effects. The adapter has 4096-byte RX/TX wire queues, sends at most one
256-byte SPP packet at a time, and waits for write/congestion events without
blocking the loop. Overflow, a write error, or a five-second stalled write or
congestion invalidates the session and requests disconnect. Partial input and
pending replies are cleared between clients, even on a rapid reconnect.
Clients should wait for query replies rather than flood metadata requests.

The Stars effect uses elapsed-time scheduling for its minimum 100 ms frame
interval and inter-cycle pause. Switching effects cancels that pause. There
are no explicit `delay()` calls in command/effect servicing.

## Hardware checks after upload

These remain manual checks; compilation and host tests do not replace them:

1. With the normal build, send `STATUS`, `BUFFERSIZE`, `SEPARATOR`,
   `PROGRAMCOUNT`, `PIXELCOUNT`, `HARDWARE`, and `ALLSTATUS` over USB and SPP.
   Expect matching values and no reply on the other connection. Reassemble
   long metadata replies using `@` continuation chunks.
2. Send split requests and `STATUS\nPIXELCOUNT\n` in a single burst. Both replies
   should be separate, ordered messages. Try a continued JSON update, then
   query `VALUES_COLORS` and inspect the LEDs.
3. Disconnect Bluetooth halfway through a command, then reconnect and send
   `STATUS`. The old partial command must not carry over. Repeat with queued
   metadata replies and a rapid reconnect; USB should remain usable.
4. Run Stars with a long pause, for example
   `{"com":7,"delay":30,"brightness":90,"values":[5,2000,0]}`. Check that queries
   and a switch to Rainbow work during its pause. Confirm the physical effect
   still fades and selects new stars as expected.
5. With the debug build, check USB startup/connection logs and free heap while
   Bluetooth serves commands. USB must not serve protocol replies in this mode.

## Host tests

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
  desktop controls, or physical LEDs. Bluetooth was still disabled at that
  checkpoint (`29582c1`).

On 2026-11-16, after adding Bluetooth and nonblocking Stars timing:

| Environment | Flash bytes / 1,310,720 | Static RAM bytes / 327,680 |
| --- | --- | --- |
| `esp32dev` | 1,168,769 (89.2%) | 56,612 (17.3%) |
| `esp32dev-usb-debug` | 1,170,061 (89.3%) | 53,300 (16.3%) |
| `esp32dev-usb-only` | 343,537 (26.2%) | 26,444 (8.1%) |

- All three builds succeeded using the same Espressif32 7.0.1,
  Arduino-ESP32 2.0.17, and FastLED 3.10.9 versions. No partition changes were
  needed; the normal Bluetooth build leaves 141,951 bytes in its app slot.
- All 25 host checks passed. As before, host sanitizer runtimes are missing;
  these checks ran without sanitizers.
- Static RAM figures do not measure runtime Bluetooth heap/stack use. The debug
  build reports current free heap for device checks.
- No board port was visible in the agent environment. No upload, pairing,
  real USB/SPP commands, physical reconnect, desktop GUI, or LED checks were
  performed for this implementation. Use the manual checks above on the board.
