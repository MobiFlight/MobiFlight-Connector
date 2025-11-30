import {
  BoardDefinition,
  calculateNamesAndLabelsForMidiController,
  JoystickDefinition,
  MidiControllerDefinition,
} from "@/types/definitions"
import { create } from "zustand"

interface ControllerDefinitionState {
  BoardDefinitions: BoardDefinition[]
  JoystickDefinitions: JoystickDefinition[]
  MidiControllerDefinitions: MidiControllerDefinition[]
  setBoardDefinitions: (definitions: BoardDefinition[]) => void
  setJoystickDefinitions: (definitions: JoystickDefinition[]) => void
  setMidiControllerDefinitions: (
    definitions: MidiControllerDefinition[],
  ) => void
}

export const useControllerDefinitionsStore = create<ControllerDefinitionState>(
  (set) => ({
    BoardDefinitions: [],
    JoystickDefinitions: [],
    MidiControllerDefinitions: [],
    setBoardDefinitions: (definitions) =>
      set({ BoardDefinitions: definitions }),
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
