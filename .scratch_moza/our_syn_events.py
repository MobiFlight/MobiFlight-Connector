import pickle, struct, sys

with open("frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

CYCLE_STARTS = [48.694, 78.055, 112.813]

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
    b0 = p[0]
    return (b0 & 0x7F, bool(b0 & 0x80), p[1:])

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

def parse_request(body):
    if len(body) < 5:
        return None
    dport = struct.unpack(">H", body[0:2])[0]
    magic = body[2]
    isn = struct.unpack("<H", body[3:5])[0]
    return dport, magic, isn, body[5:]

def parse_ack(body):
    if len(body) < 4:
        return None
    dport = struct.unpack(">H", body[0:2])[0]
    ack_isn = struct.unpack("<H", body[2:4])[0]
    return dport, ack_isn

out_ack_count = 0
out_unparsed_reply_count = 0

for time, frame, direction, inner_cmd, is_reply, inner_payload in events:
    if inner_cmd != 0x7C:
        continue
    cyc = cycle_of(time)
    if is_reply:
        r = parse_ack(inner_payload)
        if r and direction == "OUT":
            out_ack_count += 1
        print(f"[cyc{cyc}] t={time:8.3f} {direction:3s} ACK(inner reply bit set) body={inner_payload.hex()}")
    else:
        r = parse_request(inner_payload)
        if r is None:
            if direction == "OUT":
                out_unparsed_reply_count += 1
            print(f"[cyc{cyc}] t={time:8.3f} {direction:3s} UNPARSEABLE (len={len(inner_payload)}) body={inner_payload.hex()}  <-- too short to be SYN/TRANS/FIN, likely a mis-tunneled ACK")
            continue
        dport, magic, isn, body = r
        if magic in (0x80, 0x81):
            name = "SYN1" if magic == 0x80 else "SYN2"
            announced = struct.unpack("<H", body[0:2])[0] if len(body) >= 2 else None
            print(f"[cyc{cyc}] t={time:8.3f} {direction:3s} {name} dport={dport} isn={isn} announced={announced}")

print(f"\nOUT frames correctly parsed as ACK (inner reply bit set): {out_ack_count}", file=sys.stderr)
print(f"OUT frames that were too short to parse as SYN/TRANS/FIN (len<5, likely a 4-byte ACK payload sent WITHOUT the inner reply bit): {out_unparsed_reply_count}", file=sys.stderr)
