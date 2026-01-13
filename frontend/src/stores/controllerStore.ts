import { Controller } from "@/types/controller"
import { create } from "zustand"

interface ControllerState {
  controller: Controller[] | []
  setController: (controller: Controller[]) => void
}

export const useControllerStore = create<ControllerState>((set) => ({
  controller: [],
  setController: (controller) => set({ controller: controller }),
}))