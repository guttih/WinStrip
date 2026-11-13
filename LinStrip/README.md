# LinStrip

LinStrip is a Linux desktop controller for an ESP32-driven light strip. It uses
Qt 6 Widgets and communicates over USB serial at **115200 baud**. The ESP32 must
already run compatible WinStrip firmware; building this app does not flash the
board. This application currently uses serial, not Wi-Fi.

## Install dependencies

### Rocky Linux 10 / Fedora

Run in a terminal:

```bash
sudo dnf install gcc-c++ cmake make qt6-qtbase-devel qt6-qtserialport-devel
```

DNF installs the required Qt runtime libraries and development tools as
dependencies. Qt Creator and the Arduino IDE are not required to build this app.
The Qt packages are available in Rocky Linux 10's
[AppStream repository](https://download.rockylinux.org/pub/rocky/10/AppStream/x86_64/os/Packages/q/).

On the development computer inspected on 2026-11-10 (Rocky Linux 10.2), GCC,
CMake, and Make were already installed. Installing both Qt development packages
resolved the initial missing `Qt6Config.cmake` error. CMake configuration and
compilation then succeeded with Qt 6.10.1, with existing compiler warnings.
Hardware operation has not yet been verified on this computer.

## Build and run

The CMake project is in the `LinStrip` subdirectory of `WinStrip`. If your
terminal is in `~/repos/personal/WinStrip`, enter it first:

```bash
cd LinStrip
```

Then configure, build, and run:

```bash
cmake -S . -B build
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
3. Launch LinStrip, select the board's port, and click **Connect**. The app sets
   the baud rate to 115200 and requests `ALLSTATUS` from the firmware.
4. Use **Programs** to adjust lighting effects, colors, brightness, and timing.
   Use **Manual** to send commands and view their responses.

The port list is populated at startup. Restart LinStrip if you plug the board
in after opening the application.

## Troubleshooting

### CMake cannot find Qt6

Install both development packages above, then rerun `cmake -S . -B build`.
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
If the board resets when the port opens, wait for it to finish booting before
retrying a command.

### Stylesheet warning

`main.cpp` currently tries to load `customstyles.css` from an old absolute path
under `/home/gudjon/`. A missing stylesheet may produce a console warning; it
does not prevent the application from starting with Qt's default styling.
