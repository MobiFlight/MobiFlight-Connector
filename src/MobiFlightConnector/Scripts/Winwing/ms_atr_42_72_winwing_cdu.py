# ms_atr_42_72_winwing_cdu.py - entry point, runs main() from ms_atr_core
import os
import sys

# MobiFlight's embedded Python does not add the script directory to sys.path.
_here = os.path.dirname(os.path.abspath(__file__))
if _here not in sys.path:
    sys.path.insert(0, _here)

try:
    from ms_atr_core import main
except ImportError as e:
    print(f"ERROR: cannot load ms_atr_core ({e}). Requires Python 3.14 x64, found {sys.version.split()[0]}",
          file=sys.stderr)
    sys.exit(1)

if __name__ == "__main__":
    main()
