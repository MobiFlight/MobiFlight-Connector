interface IModifier {
  Type: string
  Active: boolean
}

interface Transformation extends IModifier {
  Expression: string
}

interface Substring extends IModifier {
  Start: number
  End: number
}

interface Padding extends IModifier {
  Character: string
  Length: number
  Direction: "Left" | "Right" | "Centered"
}

interface Interpolation extends IModifier {
  Values: {
    [Key: double]: double
  }
  Count: number
  Max: number
  Min: number
}

