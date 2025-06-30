import {
  calculateNamesAndLabelsForMidiController,
  JoystickDefinition,
  MidiControllerDefinition,
} from "@/types/definitions"
import { create } from "zustand"

interface ControllerDefinitionState {
  JoystickDefinitions: JoystickDefinition[]
  MidiControllerDefinitions: MidiControllerDefinition[]
  setJoystickDefinitions: (definitions: JoystickDefinition[]) => void
  setMidiControllerDefinitions: (
    definitions: MidiControllerDefinition[],
  ) => void
}

export const useControllerDefinitionsStore = create<ControllerDefinitionState>(
  (set) => ({
    JoystickDefinitions: [],
    MidiControllerDefinitions: [],
    setJoystickDefinitions: (definitions) =>
      set({ JoystickDefinitions: definitions }),
    setMidiControllerDefinitions: (definitions) => {
      const extendedDefinitions = definitions.map((def) => {
        const processedLabels = calculateNamesAndLabelsForMidiController(def)
        return { ...def, ProcessedLabels: processedLabels }
      })
      set({ MidiControllerDefinitions: extendedDefinitions })
    },
  }),
)
