import SevenSegmentDigit from "@/components/wizard/OutputConfig/OutputDevices/7segment/SevenSegmentDigit"
import { cn } from "@/lib/utils"
import { useState } from "react"
import { useTranslation } from "react-i18next"

type SegmentButtonProps = {
  value: string
  position: number
  digitActive: boolean
  decimalPointActive: boolean
  onClickSegment: () => void
  onClickDecimalPoint: () => void
} & React.ButtonHTMLAttributes<HTMLButtonElement>

const useHighlight = () => {
  const [hovered, setHovered] = useState(false)
  const [focused, setFocused] = useState(false)
  return {
    highlighted: hovered || focused,
    handlers: {
      onPointerEnter: () => setHovered(true),
      onPointerLeave: () => setHovered(false),
      onFocus: (e: React.FocusEvent<HTMLElement>) =>
        setFocused(e.currentTarget.matches(":focus-visible")),
      onBlur: () => setFocused(false),
    },
  }
}

export const SevenSegmentButton = ({
  value,
  position,
  digitActive,
  decimalPointActive,
  onClickSegment,
  onClickDecimalPoint,
  className,
  ...props
}: SegmentButtonProps) => {
  const { t } = useTranslation("wizard")
  const segmentHighlight = useHighlight()
  const decimalPointHighlight = useHighlight()

  return (
    <div className="flex flex-col gap-1">
      <div className="bg-foreground relative">
        <button
          onClick={onClickSegment}
          type="button"
          role="checkbox"
          className={cn("group h-15 w-10 bg-gray-900 px-0.5 py-1", className)}
          data-i={position}
          aria-checked={digitActive}
          aria-label={t("ConfigWizard.Device.SevenSegment.Digit", { position })}
          title={t("ConfigWizard.Device.SevenSegment.ToggleDigit", {
            position,
          })}
          {...segmentHighlight.handlers}
          {...props}
        >
          <SevenSegmentDigit
            value={value}
            digitActive={digitActive}
            dpActive={decimalPointActive}
            digitHighlighted={segmentHighlight.highlighted}
            dpHighlighted={decimalPointHighlight.highlighted}
          />
        </button>
        <button
          type="button"
          role="checkbox"
          onClick={onClickDecimalPoint}
          {...decimalPointHighlight.handlers}
          aria-checked={decimalPointActive}
          aria-label={t("ConfigWizard.Device.SevenSegment.DecimalPoint", {
            position,
          })}
          title={t("ConfigWizard.Device.SevenSegment.ToggleDecimalPoint", {
            position,
          })}
          className="group absolute right-0 bottom-0 py-2.5 pl-4"
        ></button>
      </div>
      {/* extra mouse target for the digit; keyboard and screen readers use the digit checkbox */}
      <button
        aria-hidden="true"
        onClick={onClickSegment}
        className="px-0.5"
        title={t("ConfigWizard.Device.SevenSegment.ToggleDigit", {
          position,
        })}
        {...segmentHighlight.handlers}
      >
        <div
          className={cn(
            "h-2 w-full rounded-2xl",
            digitActive ? "bg-primary" : "bg-muted-foreground/50",
          )}
        ></div>
      </button>
    </div>
  )
}
