import pickle, struct, sys
from collections import defaultdict

with open("moza_frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

CYCLE_STARTS = [12.825, 54.957, 93.076]

def cycle_of(t):
    idx = 0
    for i, s in enumerate(CYCLE_STARTS):
        if t >= s:
            idx = i
    return idx + 1

def unwrap_tunnel(frame):
    if frame["command"] not in (0x43, 0xC3):
        return None
    p = frame["payload"]
    if len(p) < 1:
        return None
    inner_cmd_byte = p[0]
    is_reply = bool(inner_cmd_byte & 0x80)
    inner_cmd = inner_cmd_byte & 0x7F
    inner_payload = p[1:]
    return inner_cmd, is_reply, inner_payload

events = []
for f in out_frames:
    u = unwrap_tunnel(f)
    if u:
        events.append((f["time"], f["frame"], "OUT", *u))
for f in in_frames:
    u = unwrap_tunnel(f)
    if u:
        events.append((f["time"], f["frame"], "IN", *u))
events.sort(key=lambda e: e[0])

STREAM_INNER = 0x7C

def parse_request(body):
    if len(body) < 5:
        return None
    dport = struct.unpack(">H", body[0:2])[0]
    magic = body[2]
    isn = struct.unpack("<H", body[3:5])[0]
    rest = body[5:]
    return dport, magic, isn, rest

def parse_ack(body):
    if len(body) < 4:
        return None
    dport = struct.unpack(">H", body[0:2])[0]
    ack_isn = struct.unpack("<H", body[2:4])[0]
    return dport, ack_isn

PORT_NAMES = {9010: "telemetry(9010)", 9020: "settings(9020)", 9050: "mcdu(9050)", 9030: "font(9030)", 9051: "mcdu-udp(9051)"}

app_bytes = defaultdict(bytearray)
timeline = []
syn_events = []  # (time, frame, cyc, dir, dport, magic_name, isn, announced_port, version)

for time, frame, direction, inner_cmd, is_reply, inner_payload in events:
    if inner_cmd != STREAM_INNER:
        continue
    cyc = cycle_of(time)
    if is_reply:
        r = parse_ack(inner_payload)
        if not r:
            continue
        dport, ack_isn = r
        pname = PORT_NAMES.get(dport, f"port{dport}")
        timeline.append((time, frame, cyc, direction, f"ACK {pname} ackIsn={ack_isn}"))
    else:
        r = parse_request(inner_payload)
        if not r:
            continue
        dport, magic, isn, body = r
        pname = PORT_NAMES.get(dport, f"port{dport}")
        if magic == 0x80 or magic == 0x81:
            name = "SYN1" if magic == 0x80 else "SYN2"
            announced = struct.unpack("<H", body[0:2])[0] if len(body) >= 2 else None
            ver = body[3] if len(body) >= 4 else None
            timeline.append((time, frame, cyc, direction, f"{name} {pname} isn={isn} announced={announced} ver={ver} body={body.hex()}"))
            syn_events.append((time, frame, cyc, direction, dport, name, isn, announced, ver))
        elif magic == 0x00:
            timeline.append((time, frame, cyc, direction, f"FIN {pname} isn={isn}"))
        elif magic == 0x01:
            if len(body) >= 4:
                app_chunk = body[:-4]
                crc = body[-4:]
            else:
                app_chunk = body
                crc = b""
            if app_chunk:
                app_bytes[(cyc, direction, dport)] += app_chunk
            timeline.append((time, frame, cyc, direction, f"TRANS {pname} isn={isn} len={len(app_chunk)} crc={crc.hex()}"))
        else:
            timeline.append((time, frame, cyc, direction, f"UNKNOWN magic=0x{magic:02x} {pname} isn={isn} body={body.hex()}"))

with open("moza_timeline.txt", "w") as f:
    for time, frame, cyc, direction, desc in timeline:
        f.write(f"[cyc{cyc}] t={time:8.3f} frame={frame:7d} {direction:3s} {desc}\n")

with open("moza_appbytes.pkl", "wb") as f:
    pickle.dump(dict(app_bytes), f)

print("=== SYN events (port announcements) ===", file=sys.stderr)
for time, frame, cyc, direction, dport, name, isn, announced, ver in syn_events:
    print(f"  [cyc{cyc}] t={time:8.3f} {direction:3s} {name} dport={dport}(0x{dport:04x}) isn={isn} announced={announced} ver={ver}", file=sys.stderr)

print("\n=== app_bytes sizes ===", file=sys.stderr)
for k in sorted(app_bytes.keys()):
    print(f"  cyc{k[0]} {k[1]} dport={k[2]}: {len(app_bytes[k])} bytes", file=sys.stderr)
