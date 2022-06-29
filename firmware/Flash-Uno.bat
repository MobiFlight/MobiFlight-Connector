cd ../Arduino/hardware/tools/avr/bin
avrdude.exe -C ../etc/avrdude.conf -p atmega328p -c arduino -P COM43 -b 115200 -D -Uflash:w:../../../../../firmware/mobiflight_uno_2_2_0.hex:i
