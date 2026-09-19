import pickle, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")
with open("moza_frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

CYCLE_STARTS = [12.825, 54.957, 93.076]
CYCLE_ENDS = [42.786, 80.002, 116.137]

merged = [("OUT", f) for f in out_frames] + [("IN", f) for f in in_frames]
merged.sort(key=lambda x: x[1]["time"])

for idx, (start, end) in enumerate(zip(CYCLE_STARTS, CYCLE_ENDS), 1):
    print(f"\n===== MOZA cycle {idx}: opening 3.0s (t={start:.3f}..{start+3.0:.3f}) =====")
    for direction, f in merged:
        t = f["time"]
        if start <= t <= start + 3.0:
            print(f"{direction} frame={f['frame']} t={t:.4f} cmd=0x{f['command']:02x} dp=0x{f['devicepair']:02x} len={f['length']} payload={f['payload'].hex()}")
