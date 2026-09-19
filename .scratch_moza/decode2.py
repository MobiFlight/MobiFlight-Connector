import pickle, struct, zlib, sys, io

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

with open("appbytes.pkl", "rb") as f:
    app_bytes = pickle.load(f)

MCDU_PORT = {1: 3, 2: 3, 3: 4}
SETTINGS_PORT = {1: 2, 2: 2, 3: 2}
TELEMETRY_PORT = {1: 1, 2: 1, 3: 1}

def networkpackages(data):
    pkgs = []
    i = 0
    n = len(data)
    while i + 5 <= n:
        pkg_id = data[i]
        size = struct.unpack("<I", data[i+1:i+5])[0]
        if i + 5 + size > n:
            pkgs.append(("INCOMPLETE", pkg_id, size, data[i+5:]))
            break
        payload = data[i+5:i+5+size]
        pkgs.append(("OK", pkg_id, size, payload))
        i += 5 + size
    return pkgs

def decode_mcdu_body(body, flags):
    if flags & 0x08:
        if len(body) < 4:
            return None
        usize = struct.unpack(">I", body[0:4])[0]
        try:
            raw = zlib.decompress(body[4:])
        except Exception as e:
            return f"<zlib decompress failed: {e}>"
    else:
        raw = body
    # parse: TextRowCount:u8, repeat Row,SegCount,(StartCol,ColLen,Utf8Len,Utf8Bytes)
    out_rows = {}
    p = 0
    try:
        text_row_count = raw[p]; p += 1
        for _ in range(text_row_count):
            row = raw[p]; seg_count = raw[p+1]; p += 2
            for _ in range(seg_count):
                start_col = raw[p]; col_len = raw[p+1]
                utf8_len = struct.unpack("<H", raw[p+2:p+4])[0]
                p += 4
                text = raw[p:p+utf8_len].decode("utf-8", errors="replace")
                p += utf8_len
                out_rows.setdefault(row, []).append((start_col, col_len, text))
        style_row_count = raw[p]; p += 1
        style_info = []
        for _ in range(style_row_count):
            row = raw[p]; seg_count = raw[p+1]; p += 2
            for _ in range(seg_count):
                start_col = raw[p]; col_len = raw[p+1]; p += 2
                styles = raw[p:p+col_len]; p += col_len
                style_info.append((row, start_col, col_len, styles.hex()))
    except Exception as e:
        return f"<parse error: {e}> partial_rows={out_rows}"
    lines = []
    for row in sorted(out_rows.keys()):
        segs = out_rows[row]
        lines.append(f"row{row}: " + " | ".join(f"col{c}:{t!r}" for c, l, t in segs))
    return "\n".join(lines)

for cyc in (1, 2, 3):
    print(f"\n========== CYCLE {cyc} : MCDU (port {MCDU_PORT[cyc]}) ==========")
    inb = app_bytes.get((cyc, "IN", MCDU_PORT[cyc]), b"")
    outb = app_bytes.get((cyc, "OUT", MCDU_PORT[cyc]), b"")
    print(f"IN bytes={len(inb)} OUT bytes={len(outb)}")
    print("-- OUT packages (host->device) --")
    for status, pid, size, payload in networkpackages(bytes(outb)):
        if pid == 0x32:  # InitConfig
            ver, rows, cols, cellbytes, servercap, udpport, kfms = struct.unpack("<BHHHBHH", payload[:12])
            print(f"  [{status}] InitConfig(0x32) size={size}: Version={ver} Rows={rows} Cols={cols} CellBytes={cellbytes} ServerCapabilities={servercap} UdpPort={udpport} KeyframeIntervalMs={kfms}")
        elif pid in (0x30, 0x31):
            name = "Keyframe" if pid == 0x30 else "Delta"
            ver = payload[0]
            seq = struct.unpack("<I", payload[1:5])[0]
            flags = payload[5]
            off = 6
            base_seq = None
            if flags & 0x04:
                base_seq = struct.unpack("<I", payload[6:10])[0]
                off = 10
            body = payload[off:]
            decoded = decode_mcdu_body(body, flags)
            print(f"  [{status}] {name}(0x{pid:02x}) size={size}: Version={ver} Seq={seq} Flags=0x{flags:02x} BaseSeq={base_seq}")
            print(f"      content:\n" + "\n".join("      " + l for l in (decoded or "").split("\n")))
        else:
            print(f"  [{status}] pkg=0x{pid:02x} size={size} payload={payload[:40].hex()}{'...' if len(payload)>40 else ''}")
    print("-- IN packages (device->host) --")
    for status, pid, size, payload in networkpackages(bytes(inb)):
        if pid == 0x33:
            ver, clientcap, udpport, pageidx = struct.unpack("<BBHB", payload[:5])
            print(f"  [{status}] ClientCapability(0x33) size={size}: Version={ver} ClientCapabilities={clientcap} UdpListenPort={udpport} PageIndex={pageidx}")
        else:
            print(f"  [{status}] pkg=0x{pid:02x} size={size} payload={payload[:40].hex()}{'...' if len(payload)>40 else ''}")

print("\n\n========== SETTINGS per cycle ==========")
def settings_frames(data):
    # Legacy preamble/values or FF|Size|CRC32|SettingId|Data frames intermixed; just scan for
    # 0xFF marker frames and known legacy fixed-order values; print raw hex windows around 0x18/0x08/0x0B writes.
    frames = []
    i = 0
    n = len(data)
    while i < n:
        if data[i] == 0xFF and i + 5 <= n:
            size = struct.unpack("<I", data[i+1:i+5])[0]
            if i + 5 + size <= n and size >= 4:
                sid = struct.unpack("<i", data[i+5:i+9])[0]
                sdata = data[i+9:i+5+size]
                frames.append(("FF", sid, sdata))
                i += 5 + size
                continue
        i += 1
    return frames

for cyc in (1, 2, 3):
    outb = bytes(app_bytes.get((cyc, "OUT", SETTINGS_PORT[cyc]), b""))
    inb = bytes(app_bytes.get((cyc, "IN", SETTINGS_PORT[cyc]), b""))
    print(f"\n-- cycle {cyc} settings OUT (host->device), {len(outb)} bytes --")
    for kind, sid, sdata in settings_frames(outb):
        print(f"  write settingId=0x{sid:02x} ({sid}) data={sdata.hex()}")
    print(f"-- cycle {cyc} settings IN (device->host), {len(inb)} bytes, scanning for 0x18/0x13/0x01 --")
    for kind, sid, sdata in settings_frames(inb):
        if sid in (0x18, 0x13, 0x01, 0x04, 0x0A):
            print(f"  echo settingId=0x{sid:02x} ({sid}) data={sdata.hex()}")
