import pickle, struct, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

with open("frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

CYCLE_STARTS = [48.694, 78.055, 112.813]
def cycle_of(t):
    idx = 0
    for i, s in enumerate(CYCLE_STARTS):
        if t >= s: idx = i
    return idx + 1

def unwrap_tunnel(frame):
    if frame["command"] not in (0x43, 0xC3): return None
    p = frame["payload"]
    if len(p) < 1: return None
    b0 = p[0]
    return (b0 & 0x7F, bool(b0 & 0x80), p[1:])

# For cycle 1, mcdu local port = 3 (IN direction, dport field == 3 meaning device sending TO host's port3)
target_cyc = 1
target_port = 3
isns_seen = []
for f in in_frames:
    u = unwrap_tunnel(f)
    if not u: continue
    inner_cmd, is_reply, body = u
    if inner_cmd != 0x7C or is_reply: continue
    if len(body) < 5: continue
    dport = struct.unpack(">H", body[0:2])[0]
    magic = body[2]
    isn = struct.unpack("<H", body[3:5])[0]
    if dport != target_port or magic != 0x01: continue
    cyc = cycle_of(f["time"])
    if cyc != target_cyc: continue
    isns_seen.append((f["frame"], f["time"], isn, len(body) - 5))

print(f"cycle {target_cyc} port {target_port} (mcdu) IN TRANS count = {len(isns_seen)}")
from collections import Counter
c = Counter(isn for _, _, isn, _ in isns_seen)
print("ISN histogram:", dict(sorted(c.items())))
for fr, t, isn, l in isns_seen:
    print(f"  frame={fr} t={t:.3f} isn={isn} applen={l}")
