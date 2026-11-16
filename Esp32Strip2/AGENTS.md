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
- `lib/StripReader` currently reads directly from `Serial`. For Bluetooth,
  accept a `Stream` per connection and keep each connection's partial input
  separate. Route every command response, including numeric responses, through
  its requesting connection.
- Process complete messages individually, enforce the receive-buffer limit,
  and preserve the existing `@` continuation format. The original reader can
  concatenate multiple commands received in one burst and does not enforce its
  advertised limit; account for these cases when changing it.
- Keep logging separate from protocol output. Avoid blocking delays in command
  processing and effects that stall servicing either connection.
- Check flash usage after Bluetooth or dependency changes. A compilation probe
  adding Bluetooth initialization used 1,177,957 of the current 1,310,720-byte
  application slot. Review the partition layout and wireless-update needs as
  the firmware grows; compile-time RAM usage does not establish runtime heap
  availability.

## Verification

Build affected firmware and report its result and relevant memory usage. The
`test/` directory currently contains a template README, not a working test suite.
Add focused regression coverage when changing framing or command handling.
For hardware work, report which queries, control updates, connection recovery,
and LED behaviors were actually checked; compilation alone does not verify them.
