cd ../Arduino/hardware/tools/avr/bin
MODE COM18:1200,N,8,1,P
timeout 2
avrdude.exe -C ../etc/avrdude.conf -p atmega32u4 -c avr109 -P COM8 -b 57600 -D -Uflash:w:../../../../../firmware/mobiflight_micro_2_2_0.hex:i
