import json, re, sys, struct, zlib

with open("moza_bulk.json", "r", encoding="utf-8") as f:
    data = json.load(f)

events = []  # (frame_num, time_rel, direction, payload_bytes)
for pkt in data:
    src = pkt["_source"]["layers"]
    frame_raw = src["frame_raw"][0]
    usb = src["usb"]
    ep = usb.get("usb.endpoint_address")
    hdrlen = int(usb.get("usb.usbpcap_header_len", "27"))
    frame_num = int(src["frame"]["frame.number"])
    time_rel = float(src["frame"]["frame.time_relative"])
    full = bytes.fromhex(frame_raw)
    payload = full[hdrlen:]
    if not payload:
        continue
    if ep == "0x02":
        direction = "OUT"  # host->device
    elif ep == "0x82":
        direction = "IN"   # device->host
    else:
        continue
    events.append((frame_num, time_rel, direction, payload))

events.sort(key=lambda e: e[0])
print(f"Total bulk data events: {len(events)}", file=sys.stderr)

out_stream = bytearray()
in_stream = bytearray()
out_marks = []
in_marks = []
for fn, t, d, p in events:
    if d == "OUT":
        out_marks.append((len(out_stream), fn, t))
        out_stream += p
    else:
        in_marks.append((len(in_stream), fn, t))
        in_stream += p

print(f"OUT stream bytes: {len(out_stream)}  IN stream bytes: {len(in_stream)}", file=sys.stderr)

with open("moza_out_stream.bin", "wb") as f:
    f.write(out_stream)
with open("moza_in_stream.bin", "wb") as f:
    f.write(in_stream)

def find_time_for_offset(marks, offset):
    best = None
    for off, fn, t in marks:
        if off <= offset:
            best = (fn, t)
        else:
            break
    return best if best else (None, None)

def unescape_and_split_frames(stream, marks, label):
    frames = []
    i = 0
    n = len(stream)
    while i < n:
        if stream[i] != 0x7E:
            i += 1
            continue
        start = i
        pos = i + 1
        def read_unescaped_byte(p):
            if p >= n:
                return None, p
            b = stream[p]
            if b == 0x7E:
                if p + 1 < n and stream[p+1] == 0x7E:
                    return 0x7E, p + 2
                else:
                    return None, p
            return b, p + 1

        length_byte, pos2 = read_unescaped_byte(pos)
        if length_byte is None:
            i += 1
            continue
        length = length_byte
        cmd_byte, pos3 = read_unescaped_byte(pos2)
        if cmd_byte is None:
            i += 1
            continue
        dp_byte, pos4 = read_unescaped_byte(pos3)
        if dp_byte is None:
            i += 1
            continue
        payload = bytearray()
        p = pos4
        ok = True
        for _ in range(length):
            b, p = read_unescaped_byte(p)
            if b is None:
                ok = False
                break
            payload.append(b)
        if not ok:
            i += 1
            continue
        chk_byte, p_after = read_unescaped_byte(p)
        if chk_byte is None:
            i += 1
            continue
        transmitted = stream[pos:p]
        s = (0x0D + 0x7E + sum(transmitted)) & 0xFF
        valid = (s == chk_byte)
        fn, t = find_time_for_offset(marks, start)
        frames.append({
            "offset": start, "frame": fn, "time": t,
            "length": length, "command": cmd_byte, "devicepair": dp_byte,
            "payload": bytes(payload), "checksum_ok": valid,
        })
        i = p_after
    return frames

out_frames = unescape_and_split_frames(out_stream, out_marks, "OUT")
in_frames = unescape_and_split_frames(in_stream, in_marks, "IN")
print(f"OUT frames: {len(out_frames)}  IN frames: {len(in_frames)}", file=sys.stderr)
bad_out = [f for f in out_frames if not f["checksum_ok"]]
bad_in = [f for f in in_frames if not f["checksum_ok"]]
print(f"OUT bad checksum: {len(bad_out)}  IN bad checksum: {len(bad_in)}", file=sys.stderr)

import pickle
with open("moza_frames.pkl", "wb") as f:
    pickle.dump({"out": out_frames, "in": in_frames}, f)
