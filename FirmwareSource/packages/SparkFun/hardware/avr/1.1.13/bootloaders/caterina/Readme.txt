Notes on building the improved 32u4 Caterina bootloader:

1. Must download LUFA-111009 and extract to this directory.

  This should be in a folder named LUFA-111009/

2. Make sure avr-gcc is installed.  It make come with tools, but
    if not maybe wingnu32 make, and core tools can be downloaded and used.

3. Try using powershell if you get "...died before initialization" errors.
  
4. The "build.txt" file and "program.txt" files can be renamed
    to .bat files and run to build and program the boards listed
    herein (Windows only, unfortunately).