export interface IModifier {
  Type: string
  Active: boolean
}

export type ModifierList = {
  Items: Modifier[]
}

export type Modifier = Transformation | Substring | Padding | Interpolation | Comparison | Blink

export interface Transformation extends IModifier {
  Type: "Transformation"
  Expression: string
}

export interface Substring extends IModifier {
  Type: "Substring"
  Start: number
  End: number
}

export interface Padding extends IModifier {
  Type: "Padding"
  Length: number
  PadChar: string
  Direction: "Left" | "Right" | "Centered"
}

export interface Interpolation extends IModifier {
  Type: "Interpolation"
  Values: Record<number, number>
}

export interface Comparison extends IModifier {
  Type: "Comparison"
  Operand: "=" | "!=" | "<" | ">" | "<=" | ">="
  Value: string
  IfValue: string
  ElseValue: string
}

export interface Blink extends IModifier {
  Type: "Blink"
  BlinkValue: string
  OnOffSequence: number[]
}

export const MODIFIER_TYPES: string[] = [
  "Transformation",
  "Substring",
  "Padding",
  "Interpolation",
  "Comparison",
  "Blink",
]

export class ModifierFactory {
  static createModifier(type: string): Modifier {
    switch (type) {
      case "Transformation":
        return {
          Type: "Transformation",
          Active: true,
          Expression: "",
        } as Transformation
      case "Substring":
        return {
          Type: "Substring",
          Active: true,
          Start: 0,
          End: 1,
        } as Substring
      case "Padding":
        return {
          Type: "Padding",
          Active: true,
          Length: 1,
          PadChar: " ",
          Direction: "Right",
        } as Padding
      case "Interpolation":
        return {
          Type: "Interpolation",
          Active: true,
          Values: {},
        } as Interpolation
      case "Comparison":
        return {
          Type: "Comparison",
          Active: true,
          Operand: "=",
          Value: "",
          IfValue: "",
          ElseValue: "",
        } as Comparison
      case "Blink":
        return {
          Type: "Blink",
          Active: true,
          BlinkValue: "",
          OnOffSequence: [500, 500],
        } as Blink
      default:
        throw new Error(`Unknown modifier type: ${type}`)
    }
  }
}
