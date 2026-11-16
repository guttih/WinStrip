# WinStrip repository guidance

## Development focus

- Development takes place on Linux. `Esp32Strip2/` is the primary active
  project: the ESP32 firmware that controls the LED strip. Its usual editor is
  VS Code with PlatformIO, opened directly in that directory.
- `LinStrip/` is the active Linux desktop controller, developed with Qt Creator
  by opening `LinStrip/CMakeLists.txt`. The same directory is also opened in
  VS Code for Codex questions and code changes.
- `WinStrip/` is the existing Windows controller. `Esp32Strip/` is the older
  firmware project. Use them as references when useful; direct new firmware
  development to `Esp32Strip2/`.
- Leave Windows application code, tests, installer, and solution files unchanged
  during Linux development (`WinStrip/`, `WinStripTests/`, `Setup/`, and
  `WinStrip.sln`). Windows changes are deferred until the user is developing on
  a Windows computer. Record any Windows follow-up needs in shared documentation.
- Read a project's own `AGENTS.md` before editing its files, including when
  working from the repository root or crossing into the sibling project.
- Separate Codex sessions may work on these projects at the same time. They
  share one Git working tree. Inspect existing changes and preserve work from
  the user and other sessions.
- Use a feature branch for development and small commits as recovery checkpoints.
  All project directories and editor windows share the same active branch;
  coordinate branch changes across active sessions.

## Development order and reuse

1. Lead feature development in `Esp32Strip2/`, maintaining one firmware that can
   serve both Linux and Windows desktop controllers.
2. Integrate firmware features with `LinStrip/` as they become available and
   verify the complete Linux workflow before starting Windows application work.
3. Develop and verify the Windows application later, on a Windows computer.

- Reuse useful Windows algorithms, data formats, and behavior when implementing
  Linux features. Candidate references include `StepGenerator`, `Theme`, `Step`,
  the transfer objects, and their tests. Port focused logic to C++ when needed
  and use reviewed inputs and expected results to check equivalent behavior.
- Prefer shared protocol documentation, compatible theme files, and reusable
  test data initially. The Windows app uses C#/.NET Framework 4.7.2, while
  LinStrip uses Qt/C++ and the firmware uses Arduino/C++; their existing code
  cannot be combined into one library without adaptation.
- Reconsider a compiled shared library when concrete, stable duplication makes
  it worthwhile. Keep any extracted core independent of GUI, operating-system,
  and hardware APIs, and account for C# interop and build/packaging costs.

## Shared communication contract

- Preserve USB serial support and existing Windows compatibility when changing
  communication. The current USB baud rate is 115200.
- The existing protocol uses case-sensitive text queries such as `STATUS`,
  `HARDWARE`, and `ALLSTATUS`, plus JSON updates for effects, brightness, timing,
  parameters, and colors. Requests end with LF; firmware replies use CRLF.
  A trailing `@` before a line ending continues the same logical message.
- In JSON updates, `com` identifies a lighting effect. Lighting effect IDs and
  counts are independent of the communication-command enum and its count.
- For protocol changes, inspect both `Esp32Strip2/src/main.cpp` and
  `LinStrip/serialcommands.h`, along with the relevant readers and serializers.
  Update affected documentation and explain any change needed in the other
  application. Coordinate changes to shared files across active sessions.

## Agreed direction

- The user confirmed the module is ESP32-WROOM-32 on 2026-11-16. Firmware now
  enables Bluetooth Classic SPP as `WinStrip` alongside USB, using Arduino
  `BluetoothSerial` for initialization/pairing and buffered SPP events/writes.
  Compilation and host tests pass; actual Bluetooth/LED hardware checks remain.
- LinStrip's Bluetooth client is still pending and should use Qt
  `QBluetoothSocket`. The configured `esp32dev` target alone is not proof of a
  different physical module; a CP2102 identifies only the USB bridge. Reassess
  the Bluetooth transport for other ESP32 variants.
- Normal firmware serves the same protocol on USB and Bluetooth. The explicit
  `esp32dev-usb-debug` build reserves USB for logs and accepts commands over
  Bluetooth; `esp32dev-usb-only` provides USB without the Bluetooth stack.
  See [the protocol contract](Esp32Strip2/protocol.md) for framing and lifecycle.
- Share command handling and message framing across USB and Bluetooth, with
  separate receive buffers per connection and replies sent to the requester.
  Keep diagnostics out of protocol replies and animations responsive.
- Prioritize reliable framing, bounded buffers, connection recovery, and effect
  metadata that lets the Linux controller evolve with the firmware.

## Verification and documentation

- Use each project's build instructions for changes affecting it. For framing
  changes, cover split messages, multiple messages in one read, continuation
  chunks, and incomplete or oversized input. For connection changes, cover
  disconnect and reconnect behavior where hardware is available.
- Distinguish successful compilation from actual device, Bluetooth, and LED
  verification. State any environment restrictions that prevent hardware checks.
- Keep this guidance and the relevant project documentation current when the
  workflow or communication contract changes.
