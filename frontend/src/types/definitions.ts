export class JoystickDefinition {
  InstanceName!: string
  Inputs?: JoystickControllerInput[]
  Outputs?: JoystickControllerOutput[]
  ProductId?: string
  VendorId?: string
  [k: string]: unknown
}

export function mapJoystickDeviceNameToLabel(
  definition: JoystickDefinition,
  name: string,
): string | undefined {
  const input = definition.Inputs?.find((input) => input.Name === name)
  if (input) {
    return input.Label
  }
  const output = definition.Outputs?.find((output) => output.Name === name)
  if (output) {
    return output.Label
  }

  return undefined
}

export function calculateNamesAndLabelsForMidiController(
  definition: MidiControllerDefinition,
): Record<string, string> {
  const processedLabels: Record<string, string> = {}

  definition.Inputs?.forEach((input) => {
    input.MessageIds.forEach((id, i) => {
      const name = input.MessageType === "Pitch" ?
        `${input.MessageType} ${input.MessageChannel}` :
        `${input.MessageType} ${input.MessageChannel}_${id}`

      const label = input.Label.replace("%", input.LabelIds[i])
      processedLabels[name] = label 
    })
  })
  return processedLabels
}

export interface JoystickControllerInput {
  Id: string
  Label: string
  Name: string
  Type: "Button" | "Axis" 
  [k: string]: unknown
}

export interface JoystickControllerOutput {
  Id: string
  Name: string
  Label: string
  Byte: number
  Bit: number
  [k: string]: unknown
}

export interface MidiControllerDefinition {
  InstanceName: string
  DifferingOutputName?: string
  EncoderNeutralPosition: number
  InitialLayer?: string
  Inputs?: MidiControllerInput[]
  Outputs?: MidiControllerOutput[]
  // these are enriched on the frontend side
  // see the definition store for the logic
  ProcessedLabels?: Record<string, string>
}

export interface MidiControllerInput {
  /**
   * Friendly label for the input. Required.
   */
  Label: string
  LabelIds: string[]
  /**
   * Associated layer for the input. Optional.
   */
  Layer?: string
  /**
   * The input's type. Supported values: Button, EndlessKnob, LimitedKnob, Fader, Pitch. Required.
   */
  InputType: "Button" | "EndlessKnob" | "LimitedKnob" | "Fader" | "Pitch"
  /**
   * The midi message type. Supported values: Note, CC, Pitch. Required.
   */
  MessageType: "Note" | "CC" | "Pitch"
  /**
   * The midi message channel. Possible value range from 1 to 16. Required.
   */
  MessageChannel: number
  /**
   * The midi message ids. Possible value range from 0 to 127. Required.
   */
  MessageIds: number[]
}

export interface MidiControllerOutput {
  /**
   * Friendly label for the output. Required.
   */
  Label: string
  LabelIds: string[]
  /**
   * Associated layer for the output. Optional.
   */
  Layer?: string
  /**
   * The midi message type. Supported values: Note, CC. Required.
   */
  MessageType: "Note" | "CC"
  /**
   * The midi message channel. Possible value range from 1 to 16. Required.
   */
  MessageChannel: number
  /**
   * The midi message ids. Possible value range from 0 to 127. Required.
   */
  MessageIds: number[]
  /**
   * Midi message value for turning on the LED. Required.
   */
  ValueOn: number
  /**
   * Midi message value for putting LED to blink mode. Optional.
   */
  ValueBlinkOn?: number
  /**
   * Midi message value for turning off the LED. Required.
   */
  ValueOff: number
  /**
   * Label of related input. When related input is triggered, output is auto refreshed. Optional.
   */
  RelatedInputLabel?: string
  /**
   * Label ids of related input, replacing the % in the related input label. Optional.
   */
  RelatedIds?: string[]
}