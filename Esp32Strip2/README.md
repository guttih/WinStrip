# LinStrip
A Linux application to control a light strip via USB and esp32.

# Get access to USB ports

To be able to program the esp32 via serial port you will need to give the 
following commands and log off and then log on again.
```
  sudo usermod -a -G tty <your_username>
  sudo usermod -a -G dialout <your_username>
```
æ