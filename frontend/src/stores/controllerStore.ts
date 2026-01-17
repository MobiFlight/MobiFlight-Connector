import { Controller } from "@/types/controller"
import { create } from "zustand"

interface ControllerState {
  controllers: Controller[] | []
  setControllers: (controllers: Controller[]) => void
}

export const useControllerStore = create<ControllerState>((set) => ({
  controllers: [],
  setControllers: (controllers) => set({ controllers: controllers }),
}))