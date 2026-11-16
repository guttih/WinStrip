# LinStrip

LinStrip is a Linux desktop controller for an ESP32-driven light strip. It uses
Qt 6 Widgets and communicates over USB serial at **115200 baud** or Bluetooth
Classic SPP (RFCOMM). The ESP32 must already run compatible WinStrip firmware;
building this app does not flash the board. The confirmed ESP32-WROOM-32
firmware advertises Bluetooth as **WinStrip**.

## Install dependencies

### Rocky Linux 10 / Fedora

Run in a terminal:

```bash
sudo dnf install gcc-c++ cmake make qt6-qtbase-devel qt6-qtserialport-devel qt6-qtconnectivity-devel
```

DNF installs the required Qt runtime libraries and development tools as
dependencies. Qt Creator and the Arduino IDE are not required to build this app.
The Qt packages are available in Rocky Linux 10's
[AppStream repository](https://download.rockylinux.org/pub/rocky/10/AppStream/x86_64/os/Packages/q/).

On the development computer inspected on 2026-11-10 (Rocky Linux 10.2), GCC,
CMake, and Make were already installed. Installing the Qt base and serial
development packages resolved the initial missing `Qt6Config.cmake` error. CMake configuration and
compilation then succeeded with Qt 6.10.1, with existing compiler warnings.
The user subsequently confirmed USB control with the previous desktop version.
The Bluetooth integration below still needs physical device verification.

## Build and run

The CMake project is in the `LinStrip` subdirectory of `WinStrip`. If your
terminal is in `~/repos/personal/WinStrip`, enter it first:

```bash
cd LinStrip
```

Then configure, build, and run:

```bash
cmake -S . -B build -DCMAKE_BUILD_TYPE=Debug
cmake --build build --parallel
./build/LinStrip
```

Run the application from a graphical desktop session as your normal user.
The system Qt installation should be found automatically; a hardcoded
`CMAKE_PREFIX_PATH` is not normally needed. These commands reuse the build
directory without deleting it.

## Edit the forms visually (optional)

Qt is already installed by the build dependencies. For a visual form editor,
install Qt Designer, available in Rocky Linux 10's AppStream repository:

```bash
sudo dnf install qt6-designer
```

Launch Qt Designer from the desktop application menu and open a source form:

- `mainwindow.ui`: main window and connection controls.
- `FormPrograms.ui`: lighting program controls.
- `FormCommands.ui`: manual command controls.

Save the `.ui` file, rebuild with `cmake --build build --parallel`, and relaunch
the app. Edit the source `.ui` files, not the generated headers under `build/`.
New controls may also need behavior implemented in the corresponding `.cpp`
file. Qt Creator is an optional full IDE; it is not needed just to edit forms.

## Debug the application

### Qt Creator (graphical IDE)

Install the IDE from EPEL (already enabled on the development computer):

```bash
sudo dnf install qt-creator
```

From the `LinStrip` directory, open the project:

```bash
qtcreator CMakeLists.txt
```

Choose a desktop kit using the system Qt 6 and GCC, and select a **Debug** build
configuration. A separate build directory such as `build-qtcreator` keeps the
IDE's build settings independent of the terminal build. Open `.ui` files to
edit forms in the integrated designer. Set a breakpoint inside a slot by
clicking beside its source line, then start debugging with **F5** and trigger
the slot using the app's controls. Breakpoints on `emit` lines let you inspect
the outgoing signal arguments.

If a desktop kit is not detected, check Qt Creator's **Preferences > Kits**:
the system compiler is `/usr/bin/g++`, GDB is `/usr/bin/gdb`, and Qt's qmake is
`/usr/lib64/qt6/bin/qmake`.

### GDB from a terminal

GDB was already installed on the development computer. To explicitly build with
debug symbols, run from `LinStrip`:

```bash
cmake -S . -B build -DCMAKE_BUILD_TYPE=Debug
cmake --build build --parallel
gdb ./build/LinStrip
```

At the GDB prompt, `break main` sets a breakpoint and `run` starts the app.
Use `next` to step over a line, `step` to step into a function, `continue` to
resume, `print variableName` to inspect a variable, and `bt` for a stack trace.
Use `quit` to exit. Run from a graphical desktop session so the app can display
its windows. Qt Designer edits layouts; it is not a debugger.

## Connect the ESP32

1. Connect the ESP32 using a USB cable that supports data. Keep the light strip
   connected to its intended power supply.
2. Before launching LinStrip, check for a serial device:

   ```bash
   ls -l /dev/ttyUSB* /dev/ttyACM*
   ```

   Depending on the board, its name may be `ttyUSB0` or `ttyACM0`. An error for
   one unmatched pattern is harmless if the other lists the board.
3. Launch LinStrip, select **USB**, click **Refresh / Discover**, select the
   board's port, and click **Connect**. The app sets 115200 baud, allows 1.5 seconds
   for a possible USB reset, and requests `ALLSTATUS` from the firmware.
4. Use **Programs** to adjust lighting effects, colors, brightness, and timing.
   Use **Manual** to send commands and view their responses.

Use **Refresh / Discover** after plugging in a board. **Connecting** offers a
**Cancel** button; **Connected** offers **Disconnect**. The program controls
become available after valid effect metadata and current settings arrive.

## Connect over Bluetooth

1. Power the ESP32 with the normal `esp32dev` firmware. Enable Bluetooth on the
   Linux computer and pair **WinStrip** using Linux Bluetooth settings. If a
   legacy PIN is requested, the firmware guide specifies `1234`.
2. In LinStrip choose **Bluetooth**, then **Refresh / Discover**. Discovery uses
   Bluetooth Classic. Select **WinStrip (address)**; the address distinguishes
   devices with the same name. The device can be selected while discovery is
   running; connecting stops discovery. **Stop discovery** ends a scan early.
3. Click **Connect**. LinStrip looks up the standard Serial Port Profile UUID
   through SDP and opens a [QBluetoothSocket](https://doc.qt.io/qt-6/qbluetoothsocket.html)
   RFCOMM connection. Pairing alone does not open this connection. Pairing approval remains in Linux settings.
4. Use the same **Programs** and **Manual** controls as with USB. Connection
   errors appear in the connection status and Manual log. After a loss or
   timeout, check power/pairing and click **Connect** again. Use **Disconnect**
   before switching back to USB.

The firmware accepts one Bluetooth client. Close other SPP clients before
connecting. The normal firmware supports USB and Bluetooth simultaneously;
updates from either client affect the same strip. Opening USB can reset the
ESP32 and drop a Bluetooth connection. The `esp32dev-usb-debug` firmware serves
protocol commands over Bluetooth only; its USB port carries diagnostics.

## Protocol and request handling

The authoritative wire contract is [Esp32Strip2/protocol.md](../Esp32Strip2/protocol.md).
`SerialTransport` manages the USB/socket lifecycle; `ProtocolSession` handles
both through `QIODevice`, and `MessageFramer` assembles LF/CRLF replies as bytes
arrive. Each reply is delivered separately, independent of the selected tab.
Trailing `@` continuation markers are removed without inserting whitespace.
The legacy literal `@` reply is recognized only for a pending `SEPARATOR` query.
Input is bounded to 3300 payload bytes. Embedded NULs and oversized messages are
discarded through the final continuation chunk, then the session is invalidated.

Queries are serialized with a bounded queue of 32 waiting commands. Known
queries sent from Manual or the external command file receive the same request
tracking as automatic queries. JSON objects have no acknowledgement; they wait
behind a pending query but never occupy a query slot themselves. Unknown text
commands and multiple commands in one manual entry are rejected locally.
The external command file still accepts one complete command per line.

A query or buffered write times out after five seconds; a Bluetooth connection
attempt times out after 30 seconds. Failed/short writes and invalid replies
that exceed framing limits disconnect the session. Pending queries, queued
updates, partial replies and debounced program changes are cleared on loss.
Reconnect explicitly to avoid attributing a late reply to a newer query (the
protocol has no request IDs). Boot text that does not match the expected reply
is logged without completing the query. Invalid `ALLSTATUS` metadata disables
program controls; retry `ALLSTATUS` from Manual after correcting the device state.
Status loading never sends updates back to the strip. Effect IDs are checked
against the received metadata array, including when its size differs from the
communication-command count.

## Host regression tests

Qt Test is supplied by `qt6-qtbase-devel`. Tests build by default; use
`-DBUILD_TESTING=OFF` to build only the application.

```bash
cmake -S . -B build -DCMAKE_BUILD_TYPE=Debug
cmake --build build --parallel
ctest --test-dir build --output-on-failure
```

CTest uses Qt's offscreen platform. Coverage includes every two-part split of
continued/multiple replies, literal `SEPARATOR`, maximum-size/oversized/NUL input,
partial replies, query serialization, unsolicited data, acknowledgement-free
updates, queue bounds, failed/short/zero writes, write/query timeouts, disconnect
cleanup, and metadata-based effect validation. A pseudo-terminal emulates a USB
device for the actual Qt serial transport and offscreen window: it verifies
handshake, manual queries across tab changes, program updates, safe status
loading, disconnect, successful reconnect, reconnect cancellation, emulated device
removal, and recoverable open errors.
`test.sh` remains an unrelated external-command experiment.

### Verification on 2026-11-16

The Debug application and test executable compile with Qt 6.10.1; all 21 Qt Test
checks pass (one CTest suite). Existing deprecation and stylesheet-loading
warnings remain. The installed
Bluetooth runtime was present but its development package was missing. For this
agent's build, the matching `qt6-qtconnectivity-devel-6.10.1-1.el10` RPM was downloaded
and extracted under `/tmp/linstrip-qt`; CMake used
`-DQt6Bluetooth_DIR=/tmp/linstrip-qt/usr/lib64/cmake/Qt6Bluetooth`. This changes no
system packages. Install the development package above for a lasting Qt Creator
or terminal setup; after installing it, remove the temporary CMake override with
`cmake -UQt6Bluetooth_DIR -UCMAKE_PREFIX_PATH -S . -B build`.

Host and offscreen tests do not verify desktop appearance, real USB resets,
Bluetooth pairing/SDP/RFCOMM, radio loss/reconnect, or LEDs. The agent environment
has no visible USB board and Qt reports no usable BlueZ service. The following
physical checks remain:

1. On USB, refresh ports, connect, inspect `ALLSTATUS`, adjust effects, colors,
   brightness and timing, disconnect/reconnect, and unplug/replug the cable.
2. Pair WinStrip, discover it, connect over SPP, and run `STATUS`, `SEPARATOR`,
   `PROGRAMCOUNT`, `PROGRAMINFO`, `ALLSTATUS`, `HARDWARE`, and `PIXELCOUNT`.
   Switch tabs while waiting for replies. Check physical effects and colors.
3. Disconnect/reconnect Bluetooth, power-cycle the ESP32 while connected, turn
   the local adapter off/on, cancel connecting, and retry after a timeout.
4. With both clients connected, verify replies reach only the requester and USB
   remains usable. Recheck firmware response during long Stars pauses.

No firmware change or Windows application change is required for this client.
Firmware documentation still describes the desktop Bluetooth client as pending;
update those historical/current-state notes in its session after integration is
verified. Windows framing and Bluetooth remain deferred to Windows development.

## Troubleshooting

### CMake cannot find Qt6

Install all development packages above, then rerun `cmake -S . -B build`.
Installing only Qt runtime packages is insufficient. If you installed Qt in a
custom location, supply its actual installation prefix with
`-DCMAKE_PREFIX_PATH=/path/to/Qt`.

### No serial device appears

Try another data-capable USB cable or USB port. Compare the device listing
before and after plugging in the ESP32. Linux normally supplies the USB serial
drivers; identify the board's USB adapter before installing additional drivers.

### Permission denied when connecting

Inspect the device's ownership and your current groups (replace the example
device with the one actually present):

```bash
ls -l /dev/ttyUSB0
id -nG
```

If the device is writable by the `dialout` group and you are not a member, add
your user:

```bash
sudo usermod -aG dialout "$USER"
```

Log out of the desktop completely and log back in before retrying. If the
device uses another group, check the local serial-device permissions before
changing group membership. Do not run the GUI with `sudo` or make the device
world-writable.

### Port opens but controls do not work

Close any serial monitor or other application using the port. Check that you
selected the ESP32's port and that its firmware supports this application's
commands. In **Manual**, send `STATUS` or `ALLSTATUS` and inspect the response.
LinStrip allows a short boot interval when opening USB. If the initial query
times out, reconnect after the board has finished booting.

### Bluetooth discovery or connection fails

Check that Linux Bluetooth settings can see and pair WinStrip, the adapter is
powered, and the BlueZ service is running. Install `qt6-qtconnectivity-devel` if
CMake cannot find `Qt6Bluetooth`. LinStrip displays discovery/socket errors in the
status area and Manual log. Refresh after enabling the adapter and reconnect.
A paired device may still lack a usable SPP service if the wrong firmware was
uploaded or another client holds the single SPP connection.


### Stylesheet warning

`main.cpp` still loads `customstyles.css` from an old absolute path under
`/home/gudjon/`. If it is missing, Qt uses default styling. This pre-existing
styling issue does not prevent USB or Bluetooth control.
