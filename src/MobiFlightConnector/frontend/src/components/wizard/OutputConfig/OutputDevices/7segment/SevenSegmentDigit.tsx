import { cn } from "@/lib/utils"

const digitSegments = ["a", "b", "c", "d", "e", "f", "g"]

const digitSegmentsPolygonPoints = {
  a: "12,8 16,4 40,4 44,8 40,12 16,12",
  b: "46,10 50,14 50,44 46,48 42,44 42,14",
  c: "46,52 50,56 50,86 46,90 42,86 42,56",
  d: "12,92 16,88 40,88 44,92 40,96 16,96",
  g: "12,50 16,46 40,46 44,50 40,54 16,54",
  e: "10,52 14,56 14,86 10,90 6,86 6,56",
  f: "10,10 14,14 14,44 10,48 6,44 6,14",
} as Record<string, string>

const valueToSegments = {
  "0": ["a", "b", "c", "d", "e", "f"],
  "1": ["b", "c"],
  "2": ["a", "b", "g", "e", "d"],
  "3": ["a", "b", "g", "c", "d"],
  "4": ["f", "g", "b", "c"],
  "5": ["a", "f", "g", "c", "d"],
  "6": ["a", "f", "g", "e", "c", "d"],
  "7": ["a", "b", "c"],
  "8": ["a", "b", "c", "d", "e", "f", "g"],
  "9": ["a", "b", "c", "d", "f", "g"],
} as Record<string, string[]>

type SevenSegmentDigitProps = {
  value: string
  digitActive: boolean
  dpActive: boolean
  digitHighlighted: boolean
  dpHighlighted: boolean
}

const colorVariants = {
  active: {
    default: "bg-red-600 fill-red-600 shadow-md",
    highlighted: "fill-red-500 bg-red-500",
    blur: {
      segment:
        "[filter:drop-shadow(0_0_2px_var(--color-red-500))_drop-shadow(0_0_10px_var(--color-red-500))]",
      dp: "[filter:drop-shadow(0_0_2px_var(--color-red-500))_drop-shadow(0_0_3px_var(--color-red-500))]",
    },
  },
  inactive: {
    default: "bg-red-950 fill-red-950 ",
    highlighted: "fill-red-800/70 bg-red-800/70",
  },
}

const segmentColor = (
  active: boolean,
  highlighted: boolean,
  mode: "segment" | "dp",
) => {
  if (active) {
    return cn(
      colorVariants["active"]["default"],
      highlighted && colorVariants["active"]["highlighted"],
      mode && colorVariants["active"]["blur"][mode],
    )
  } else {
    return cn(
      colorVariants["inactive"]["default"],
      highlighted && colorVariants["inactive"]["highlighted"],
    )
  }
}

const SevenSegmentDigit = ({
  value,
  digitActive,
  dpActive,
  digitHighlighted,
  dpHighlighted,
}: SevenSegmentDigitProps) => {
  const activeSegments = valueToSegments[value] || []
  const isSegmentActive = (segment: string) => activeSegments.includes(segment)

  return (
    <div className="relative">
      <svg viewBox="-12 0 76 100" aria-hidden="true">
        <g transform="skewX(-6)">
          {digitSegments.map((segment) => (
            <polygon
              key={segment}
              className={cn(
                segmentColor(
                  isSegmentActive(segment) && digitActive,
                  digitHighlighted,
                  "segment",
                ),
              )}
              data-s={segment}
              points={digitSegmentsPolygonPoints[segment]}
            ></polygon>
          ))}
        </g>
      </svg>
      <div
        className={cn(
          "absolute right-0 bottom-0 h-1.5 w-1.5 rounded-full",
          segmentColor(dpActive, dpHighlighted, "dp"),
        )}
      ></div>
    </div>
  )
}
export default SevenSegmentDigit
