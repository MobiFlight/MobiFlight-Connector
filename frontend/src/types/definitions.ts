export class JoystickDefinition {
  InstanceName!: string
  Inputs?: ControllerInput[]
  Outputs?: ControllerOutput[]
  ProductId?: string
  VendorId?: string
  [k: string]: unknown  
}

export function mapJoystickNameToLabel(definition: JoystickDefinition, name: string) : string | undefined {
    const input = definition.Inputs?.find(input => input.Name === name);
    if (input) {
      return input.Label;
    }
    const output = definition.Outputs?.find(output => output.Name === name);
    if (output) {
      return output.Label;
    }

    return undefined
  }


export interface ControllerInput{
  Id: string
  Label: string
  Name: string
  Type: "Button" | "Axis"
  [k: string]: unknown
}

export interface ControllerOutput {
  Id: string
  Name: string
  Label: string
  Byte: number
  Bit: number
  [k: string]: unknown
}