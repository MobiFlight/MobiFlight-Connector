import { Controller } from "@/types/controller"
import { create } from "zustand"

interface ControllerState {
  controller: Controller[] | null
  setController: (controller: Controller[]) => void
}

export const useControllerStore = create<ControllerState>((set) => ({
  controller: null,
  setController: (controller) => set({ controller: controller }),
}))