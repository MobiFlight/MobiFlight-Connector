import { Controller, vJoyDefinition } from "@/types/controller"
import { create } from "zustand"

interface ControllerState {
  controllers: Controller[] | []
  setControllers: (controllers: Controller[]) => void
}

export const useControllerStore = create<ControllerState>((set) => ({
  controllers: [],
  setControllers: (controllers) => set({ controllers: controllers }),
}))

interface VJoyControllerState {
  vJoyDefinitions: vJoyDefinition[] | []
  setVJoyDefinitions: (vJoyDefinitions: vJoyDefinition[]) => void
}

export const useVJoyControllerStore = create<VJoyControllerState>((set) => ({
  vJoyDefinitions: [],
  setVJoyDefinitions: (vJoyDefinitions) => set({ vJoyDefinitions: vJoyDefinitions }),
}))