// src/stores/clipboardStore.ts
import { create } from "zustand"
import { Action } from "@/types/config"

interface ClipboardStore {
  action: Action | null
  copy: (action: Action) => void
  clear: () => void
}

export const useClipboardStore = create<ClipboardStore>((set) => ({
  action: null,
  copy: (action) => set({ action }),
  clear: () => set({ action: null }),
}))

export const useClipboardAction = () => useClipboardStore((s) => s.action)
export const useClipboardCopy = () => useClipboardStore((s) => s.copy)
export const useClipboardClear = () => useClipboardStore((s) => s.clear)