cd ../Arduino/hardware/tools/avr/bin
avrdude.exe -C ../etc/avrdude.conf -p atmega2560 -c wiring -P COM4 -b 115200 -D -Uflash:w:../../../../../firmware/mobiflight_mega_2_2_0.hex:i
