import pickle, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")
with open("frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

# All frames merged in time order, print a window around t=49.42 to 49.6
merged = [("IN", f) for f in in_frames] + [("OUT", f) for f in out_frames]
merged.sort(key=lambda x: x[1]["time"])
for direction, f in merged:
    if 49.40 <= f["time"] <= 49.70:
        print(f"{direction} frame={f['frame']} t={f['time']:.4f} cmd=0x{f['command']:02x} dp=0x{f['devicepair']:02x} payload={f['payload'].hex()}")
