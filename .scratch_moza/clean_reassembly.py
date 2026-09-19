import pickle, struct, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

def unwrap_tunnel(frame):
    if frame["command"] not in (0x43, 0xC3): return None
    p = frame["payload"]
    if len(p) < 1: return None
    b0 = p[0]
    return (b0 & 0x7F, bool(b0 & 0x80), p[1:])

def parse_request(body):
    if len(body) < 5: return None
    dport = struct.unpack(">H", body[0:2])[0]
    magic = body[2]
    isn = struct.unpack("<H", body[3:5])[0]
    return dport, magic, isn, body[5:]

def load_events(pklfile):
    with open(pklfile, "rb") as f:
        d = pickle.load(f)
    events = []
    for f_ in d["out"]:
        u = unwrap_tunnel(f_)
        if u: events.append((f_["time"], "OUT", *u))
    for f_ in d["in"]:
        u = unwrap_tunnel(f_)
        if u: events.append((f_["time"], "IN", *u))
    events.sort(key=lambda e: e[0])
    return events

def cycle_of(t, starts):
    idx = 0
    for i, s in enumerate(starts):
        if t >= s: idx = i
    return idx + 1

def reassemble_clean(events, starts, service_port, our_direction, device_direction):
    """For each cycle: find the LAST SYN1 (from device) for service_port that got a SYN2
    reply (from us), then walk forward through TRANS frames for that local port whose ISN
    is exactly contiguous starting at syn1.isn+1 - matching what a real Reliable Stream
    implementation actually binds to and accepts, ignoring any pre-handshake backlog."""
    results = {}
    for cyc in (1, 2, 3):
        # 1. find SYN1s for this service port in this cycle
        syn1s = []
        for t, d, inner_cmd, is_reply, payload in events:
            if cycle_of(t, starts) != cyc or inner_cmd != 0x7C or is_reply: continue
            r = parse_request(payload)
            if not r: continue
            dport, magic, isn, body = r
            if dport != service_port or magic != 0x80: continue
            announced = struct.unpack("<H", body[0:2])[0] if len(body) >= 2 else None
            syn1s.append((t, isn, announced))
        if not syn1s:
            results[cyc] = (None, b"")
            continue
        # 2. find SYN2s we sent, matching one of those announced local ports
        syn2_times = {}
        for t, d, inner_cmd, is_reply, payload in events:
            if cycle_of(t, starts) != cyc or inner_cmd != 0x7C or is_reply: continue
            if d != our_direction: continue
            r = parse_request(payload)
            if not r: continue
            dport, magic, isn, body = r
            if magic != 0x81: continue
            syn2_times[dport] = t  # dport here is the local port we're replying about
        # 3. pick the syn1 whose announced local port has the LATEST syn2 (the one that stuck)
        candidates = [(t, isn, ann) for t, isn, ann in syn1s if ann in syn2_times]
        if not candidates:
            results[cyc] = (None, b"")
            continue
        t_syn1, base_isn, local_port = max(candidates, key=lambda c: syn2_times[c[2]])
        syn2_time = syn2_times[local_port]
        # 4. walk TRANS frames from device_direction, dport==local_port, isn strictly
        #    contiguous from base_isn+1, occurring at/after the syn2 (real acceptance)
        trans = []
        for t, d, inner_cmd, is_reply, payload in events:
            if d != device_direction or inner_cmd != 0x7C or is_reply: continue
            r = parse_request(payload)
            if not r: continue
            dport, magic, isn, body = r
            if dport != local_port or magic != 0x01: continue
            if t < syn2_time: continue
            trans.append((t, isn, body))
        trans.sort(key=lambda x: (x[0], x[1]))
        expected = (base_isn + 1) & 0xFFFF
        stream = bytearray()
        for t, isn, body in trans:
            if isn != expected: continue  # duplicate/backlog/out-of-order - real impl ignores it (already-seen or not-yet-expected)
            app_chunk = body[:-4] if len(body) >= 4 else body
            stream += app_chunk
            expected = (expected + 1) & 0xFFFF
        results[cyc] = (local_port, bytes(stream))
    return results

OUR_STARTS = [48.694, 78.055, 112.813]
MOZA_STARTS = [12.825, 54.957, 93.076]

our_events = load_events("frames.pkl")
moza_events = load_events("moza_frames.pkl")

print("=== OUR settings (9020) - clean reassembly ===")
our_settings = reassemble_clean(our_events, OUR_STARTS, 9020, "OUT", "IN")
for cyc, (port, stream) in our_settings.items():
    print(f"cyc{cyc} localport={port} len={len(stream)}")
    print(f"  {stream[:140].hex()}")

print()
print("=== MOZA settings (9020) - clean reassembly ===")
moza_settings = reassemble_clean(moza_events, MOZA_STARTS, 9020, "OUT", "IN")
for cyc, (port, stream) in moza_settings.items():
    print(f"cyc{cyc} localport={port} len={len(stream)}")
    print(f"  {stream[:140].hex()}")
