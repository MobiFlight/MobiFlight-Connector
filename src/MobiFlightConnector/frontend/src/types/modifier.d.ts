export interface Modifier {
  Type: string
  Active: boolean
}

export interface Transformation extends Modifier {
  Type: "Transformation"
  Expression: string
}

export interface Substring extends Modifier {
  Type: "Substring"
  Start: number
  End: number
}

export interface Padding extends Modifier {
  Type: "Padding"
  Length: number
  PadChar: string
  Direction: "Left" | "Right" | "Centered"
}

export interface Interpolation extends Modifier {
  Type: "Interpolation"
  Values: Record<number, number> 
}

export interface Comparison extends Modifier {
  Type: "Comparison"
  Operand: "=" | "!=" | "<" | ">" | "<=" | ">="
  Value: string
  IfValue: string
  ElseValue: string
}

export interface Blink extends Modifier {
  Type: "Blink"
  BlinkValue: string
  OnOffSequence: number[]
}