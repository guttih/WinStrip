# Esp32Strip2 firmware guidance

This file supplements [the repository guidance](../AGENTS.md). This directory is
the primary firmware project and is opened as its own VS Code/PlatformIO project.
Use [development.md](development.md) for environment, upload, and serial-monitor
details.

## Build

Run from this directory:

```bash
pio run -e esp32dev
```

If `pio` is not on PATH, use `~/.platformio/penv/bin/pio`. PlatformIO manages
the Arduino framework, compiler, and FastLED dependency. Build products belong
under `.pio/`; edit source under `src/`, `lib/`, and `include/`.

The current configuration is `espressif32`, board `esp32dev`, Arduino, and
FastLED. The environment verified in November 2026 uses Espressif32 7.0.1,
Arduino-ESP32 2.0.17, and FastLED 3.10.9. Dependencies are not fully pinned;
avoid mixing unrelated toolchain upgrades into feature changes.

## Hardware and firmware changes

- Hardware settings are currently in `src/main.cpp`: 62 APA102 LEDs, data GPIO
  14, clock GPIO 13, BGR order, and USB serial at 115200 baud. Treat the source
  and identified physical hardware as authoritative before uploading.
- Building does not upload firmware. For tasks involving upload or a serial
  monitor, identify the actual port and release it from LinStrip or other
  monitors first. Opening the USB serial connection can reset the board.
- `lib/StripReader/SerialReader` accepts a `Stream` and owns a fixed 3300-byte
  payload buffer per instance. Keep one instance per connection and route all
  replies, including numeric responses, through that instance's `printLine`.
- Process complete messages individually and preserve LF/CRLF and `@`
  continuation framing. Oversized or NUL-containing messages are discarded
  through their final logical-message boundary. Each read has a byte budget;
  preserve it so incomplete input cannot monopolize the loop. See [protocol.md](protocol.md).
- Call `resetBuffer()` when a connection is lost so partial input and overflow
  state cannot carry into the next session. Bluetooth lifecycle handling is
  still to be implemented; only the USB connection is currently instantiated.
- Keep logging separate from protocol output. Avoid blocking delays in command
  processing and effects that stall servicing either connection. Application
  diagnostics are currently removed; USB debug logging must be an explicit
  development option when that port is not carrying controller replies.
  The Stars effect still has blocking delays to address before enabling Bluetooth.
- Check flash usage after Bluetooth or dependency changes. A compilation probe
  adding Bluetooth initialization used 1,177,957 of the current 1,310,720-byte
  application slot. Review the partition layout and wireless-update needs as
  the firmware grows; compile-time RAM usage does not establish runtime heap
  availability.

## Verification

Build affected firmware and report its result and relevant memory usage. Run
`bash test/run_host_tests.sh` for native framing and reply-routing regression
coverage; it compiles the production reader against a minimal test Stream.
Extend this coverage when changing framing or command handling.
For hardware work, report which queries, control updates, connection recovery,
and LED behaviors were actually checked; compilation alone does not verify them.
