# LinStrip desktop guidance

This file supplements [the repository guidance](../AGENTS.md). LinStrip is a
Qt 6 Widgets application using C++17 and CMake. Open `CMakeLists.txt` in Qt
Creator and open this same directory in VS Code for Codex. Both editors share
the same files; save edits before switching and reload externally changed files
when prompted. Use [README.md](README.md) for Linux dependencies, Designer,
debugging, and serial troubleshooting.

## Build and run

Run from this directory:

```bash
cmake -S . -B build -DCMAKE_BUILD_TYPE=Debug
cmake --build build --parallel
./build/LinStrip
```

Reuse the existing build directory. Qt Creator may use its own build directory;
preserve its configuration and other active sessions' build products. Edit the
source `.ui` forms and C++ files, not generated UI or MOC headers under a build
directory. Keep new resource paths independent of the developer's home directory.

## Connections and protocol

- The implementation links Qt Core, Widgets, SerialPort, and Bluetooth.
  `SerialTransport` opens USB at 115200 or Bluetooth Classic SPP via SDP and
  `QBluetoothSocket`. On Rocky Linux, the Bluetooth development package is
  `qt6-qtconnectivity-devel`; the runtime package alone is insufficient.
- Keep effect controls independent of the selected connection. USB
  `QSerialPort` and Bluetooth `QBluetoothSocket` can share message handling
  through `QIODevice`, using asynchronous signals for data and connection state.
- Frame received messages as bytes arrive. Handle partial lines, multiple
  replies per read, and `@` continuation chunks without using elapsed time as
  the message boundary or clearing unread device data after a reply.
- Track requests, timeouts, and disconnects independently of which UI tab is
  visible. Reset pending state on connection loss; report recoverable device
  errors in the UI instead of terminating the application.
- Populate effect choices from firmware metadata. Validate the selected effect
  against the received effect list, not `SERIAL_COMMAND::COMMAND_COUNT`.
  `SerialStatus` now validates against the complete metadata array and rejects
  malformed entries rather than changing effect indexes.
- Preserve both manual commands and program controls as transports evolve.
  Provide device refresh/discovery and explicit connecting, connected, and
  disconnected states when extending connection controls.
- `ProtocolSession` serializes queries independently of tabs, limits queued
  commands, and treats JSON updates as acknowledgement-free. Query/write timeout
  or framing failure invalidates the connection; reconnect explicitly because
  replies contain no request IDs. `MessageFramer` handles the literal `@` reply
  only when `SEPARATOR` is pending. Never clear unread device data after a reply.
- Stop debounced updates and disable program controls on loss. Enable them only
  after valid `ALLSTATUS`; loading status must not send updates back to the strip.

## Verification

Build after C++ or form changes. Run `ctest --test-dir build --output-on-failure`:
`LinStripTests` covers incremental framing, request state and failures, metadata,
and the offscreen GUI with a pseudo-terminal USB device. Tests build by default
with Qt Test; `-DBUILD_TESTING=OFF` disables them. `test.sh` is an old
external-command experiment, not a test runner. Keep regression coverage for
framing and request-state changes. An offscreen Qt startup check can help in a headless session, but does not verify visual layout
or device communication. For connection or control changes, exercise the
relevant GUI and hardware when available and report any checks still unverified.
