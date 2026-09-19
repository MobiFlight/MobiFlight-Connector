import pickle, struct, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

def networkish_settings_frames(data):
    """Per guide 6.2: FF | Size:u32 LE | CRC32:u32 LE | SettingId:i32 LE | Data.
    Size=4+len(Data) -- it covers SettingId+Data only, NOT CRC32. Verified against the
    guide's own displayOffset example (7e 1f 43 12 ... ff 0c000000 d3d498f1 1b000000
    0a000000 feffffff ...): Size=0x0c=12, Data=Size-4=8 bytes = x=10,y=-2. Matches."""
    frames = []
    i = 0
    n = len(data)
    while i < n:
        if data[i] == 0xFF and i + 13 <= n:
            size = struct.unpack("<I", data[i+1:i+5])[0]
            frame_end = i + 9 + size
            if size >= 4 and frame_end <= n:
                crc32 = struct.unpack("<I", data[i+5:i+9])[0]
                setting_id = struct.unpack("<i", data[i+9:i+13])[0]
                sdata = data[i+13:frame_end]
                frames.append((i, "FF", setting_id, sdata, crc32))
                i = frame_end
                continue
        i += 1
    return frames

def label(fname, key_prefix):
    with open(fname, "rb") as f:
        app_bytes = pickle.load(f)
    for k in sorted(app_bytes.keys()):
        cyc, direction, port = k
        if port != 2:
            continue
        data = bytes(app_bytes[k])
        print(f"\n-- {key_prefix} cycle {cyc} settings {direction} ({len(data)} bytes) --")
        frames = networkish_settings_frames(data)
        for off, kind, sid, sdata, crc in frames:
            marker = "  <<<<< DISPLAYMODE" if sid == 0x18 else ("  <<<<< CABINPOSITION" if sid == 0x13 else "")
            print(f"  off={off:6d} settingId=0x{sid:02x} ({sid}) data={sdata.hex()}{marker}")

label("appbytes.pkl", "MOBIFLIGHT")
label("moza_appbytes.pkl", "MOZA")
