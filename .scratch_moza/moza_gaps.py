import pickle
with open("moza_frames.pkl", "rb") as f:
    d = pickle.load(f)
out_frames = d["out"]
in_frames = d["in"]

merged = [("OUT", f) for f in out_frames] + [("IN", f) for f in in_frames]
merged.sort(key=lambda x: x[1]["time"])

print(f"first t={merged[0][1]['time']:.3f} last t={merged[-1][1]['time']:.3f} total={len(merged)}")

prev_t = None
for direction, f in merged:
    t = f["time"]
    if prev_t is not None and t - prev_t > 2.0:
        print(f"GAP {t - prev_t:.3f}s between t={prev_t:.3f} and t={t:.3f} (next frame={f['frame']} dir={direction})")
    prev_t = t

print()
print("=== OUT root-handshake-shaped frames (len=0,cmd=0x00,dp=0x12) ===")
for f in out_frames:
    if f["length"] == 0 and f["command"] == 0x00 and f["devicepair"] == 0x12:
        print(f"  frame={f['frame']} t={f['time']:.3f}")

print("=== IN root-handshake-response-shaped frames (len=0,cmd=0x80,dp=0x21) ===")
for f in in_frames:
    if f["length"] == 0 and f["command"] == 0x80 and f["devicepair"] == 0x21:
        print(f"  frame={f['frame']} t={f['time']:.3f}")

print("=== OUT device-init-shaped frames (len=1,cmd=0x43,dp=0x12,payload=00) ===")
for f in out_frames:
    if f["length"] == 1 and f["command"] == 0x43 and f["devicepair"] == 0x12 and f["payload"] == b'\x00':
        print(f"  frame={f['frame']} t={f['time']:.3f}")

print("=== IN device-init-response-shaped frames (len=1,cmd=0xC3,dp=0x21,payload=80) ===")
for f in in_frames:
    if f["length"] == 1 and f["command"] == 0xC3 and f["devicepair"] == 0x21 and f["payload"] == b'\x80':
        print(f"  frame={f['frame']} t={f['time']:.3f}")

print()
print("=== Command/DevicePair histogram OUT ===")
from collections import Counter
c = Counter((f["command"], f["devicepair"]) for f in out_frames)
for k, v in sorted(c.items(), key=lambda x: -x[1]):
    print(f"  cmd=0x{k[0]:02x} dp=0x{k[1]:02x} : {v}")

print("=== Command/DevicePair histogram IN ===")
c = Counter((f["command"], f["devicepair"]) for f in in_frames)
for k, v in sorted(c.items(), key=lambda x: -x[1]):
    print(f"  cmd=0x{k[0]:02x} dp=0x{k[1]:02x} : {v}")
