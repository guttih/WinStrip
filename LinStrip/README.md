# LinStrip

Make sure you have the following packages installed.

```bash
   sudo dnf install qt6-qtbase-devel qt6-qtserialport qt6-qtserialport-devel
```

```bash
rm -rf build;mkdir -p build && cd build && cmake -DCMAKE_PREFIX_PATH=/usr/lib64/cmake .. && make ; cd ..
```

