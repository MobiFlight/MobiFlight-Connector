import pickle, struct, zlib, sys
from collections import defaultdict

with open("frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]  # host -> device (cmd=0x43, dp=0x12)
in_frames = d["in"]    # device -> host (cmd=0xC3/0x0e/0x80, dp=0x21)

CYCLE_STARTS = [48.694, 78.055, 112.813]

def cycle_of(t):
    idx = 0
    for i, s in enumerate(CYCLE_STARTS):
        if t >= s:
            idx = i
    return idx + 1

# ---- Step 1: unwrap 0x43/0xC3 tunnel to get inner command + inner payload ----
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

events = []  # (time, frame, direction, inner_cmd, is_reply, inner_payload)
for f in out_frames:
    u = unwrap_tunnel(f)
    if u:
        events.append((f["time"], f["frame"], "OUT", *u))
for f in in_frames:
    u = unwrap_tunnel(f)
    if u:
        events.append((f["time"], f["frame"], "IN", *u))
events.sort(key=lambda e: e[0])

print(f"Total tunnel events: {len(events)}", file=sys.stderr)
from collections import Counter
c = Counter((e[2], e[3], e[4]) for e in events)
for k, v in sorted(c.items()):
    print(f"  dir={k[0]} innerCmd=0x{k[1]:02x} isReply={k[2]} : {v}", file=sys.stderr)

# ---- Step 2: Reliable Stream reassembly per (direction-independent) port ----
# innerCmd 0x7C = SYN1/SYN2/TRANS/FIN request; isReply True on that same 0x7C means... actually
# per guide, ACK uses INNER command 0xFC = 0x7C | 0x80, i.e. innerCmd==0x7C and is_reply==True.
# A request (SYN1/SYN2/TRANS/FIN) has innerCmd==0x7C and is_reply==False in our unwrap (since the
# reply bit lives in the byte we already split into is_reply).

STREAM_INNER = 0x7C

def parse_request(body):
    # DestinationPort:u16 BE | Magic:u8 | ISN:u16 LE | Body
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

# application byte streams per (cycle, direction, port)
app_bytes = defaultdict(bytearray)
timeline = []  # (time, frame, cycle, dir, event_desc)

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
        if magic == 0x80:
            timeline.append((time, frame, cyc, direction, f"SYN1 {pname} isn={isn} body={body.hex()}"))
        elif magic == 0x81:
            timeline.append((time, frame, cyc, direction, f"SYN2 {pname} isn={isn} body={body.hex()}"))
        elif magic == 0x00:
            timeline.append((time, frame, cyc, direction, f"FIN {pname} isn={isn}"))
        elif magic == 0x01:
            # TRANS: ApplicationChunk + StreamCRC(4 bytes, v3)
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

with open("timeline.txt", "w") as f:
    for time, frame, cyc, direction, desc in timeline:
        f.write(f"[cyc{cyc}] t={time:8.3f} frame={frame:7d} {direction:3s} {desc}\n")

print(f"Timeline entries: {len(timeline)}  (written to timeline.txt)", file=sys.stderr)

with open("appbytes.pkl", "wb") as f:
    pickle.dump(dict(app_bytes), f)

for k in sorted(app_bytes.keys()):
    print(f"appbytes cyc{k[0]} {k[1]} port{k[2]}: {len(app_bytes[k])} bytes", file=sys.stderr)
