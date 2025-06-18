import { JoystickDefinition } from "@/types/definitions"
import { create } from "zustand"

interface ControllerDefinitionState {
  JoystickDefinitions: JoystickDefinition[]
  setJoystickDefinitions: (definitions: JoystickDefinition[]) => void
}

export const useControllerDefinitionsStore = create<ControllerDefinitionState>((set) => ({
  JoystickDefinitions: [],
  setJoystickDefinitions: (definitions) => set({ JoystickDefinitions: definitions })
}))