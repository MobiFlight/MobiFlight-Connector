import { cn } from "@/lib/utils"

type SegmentButtonProps = {
  value: string
  position: number
  digitActive: boolean
  decimalPointActive: boolean
  onClickSegment: () => void
  onClickDecimalPoint: () => void
} & React.ButtonHTMLAttributes<HTMLButtonElement>

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

export const SevenSegmentButton = ({
  value,
  position,
  digitActive,
  decimalPointActive,
  onClickSegment,
  onClickDecimalPoint,
  ...props
}: SegmentButtonProps) => {
  const activeSegments = valueToSegments[value] || []
  const isSegmentActive = (segment: string) => activeSegments.includes(segment)

  const colorVariants = {
    active: "bg-red-600 fill-red-600 group-hover:fill-red-500 group-hover:bg-red-500",
    inactive: "bg-red-950 fill-red-950 group-hover:fill-red-800/70 group-hover:bg-red-800/70",
  }
  return (
    <div className="flex flex-col gap-1">
      <div className="relative bg-foreground">
        <button
          onClick={onClickSegment}
          type="button"
          role="checkbox"
          className={cn("group h-12 w-8 bg-gray-900 px-0.5 py-1", props?.className)}
          data-i={position}
          aria-pressed="true"
          aria-label={`Digit ${position}, used by this value`}
          title={`Toggle digit ${position}`}
          {...props}
        >
          <svg viewBox="-12 0 76 100" aria-hidden="true">
            <g transform="skewX(-6)">
              {digitSegments.map((segment) => (
                <polygon
                  key={segment}
                  className={`${isSegmentActive(segment) && digitActive ? colorVariants["active"] : colorVariants["inactive"]}`}
                  data-s={segment}
                  points={digitSegmentsPolygonPoints[segment]}
                ></polygon>
              ))}
            </g>
          </svg>
        </button>
        <button
          onClick={onClickDecimalPoint}
          aria-label="decimal point"
          title="Toggle decimal point"
          className="group absolute -right-0.5 bottom-0.5 p-1"
        >
          <div
            className={cn(
              "h-1 w-1 rounded-full",
              decimalPointActive
                ? colorVariants["active"]
                : colorVariants["inactive"],
            )}
          ></div>
        </button>
      </div>
      <button onClick={onClickSegment} className="px-0.5">
        {digitActive ? (
          <div className="bg-primary h-1 w-full"></div>
        ) : (
          <div className="bg-muted-foreground/50 h-1 w-full"></div>
        )}
      </button>
    </div>
  )
}
