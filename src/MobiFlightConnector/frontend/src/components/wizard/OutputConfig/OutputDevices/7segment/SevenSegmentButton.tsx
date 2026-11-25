import SevenSegmentDigit from "@/components/wizard/OutputConfig/OutputDevices/7segment/SevenSegmentDigit"
import { cn } from "@/lib/utils"
import { useTranslation } from "react-i18next"

type SegmentButtonProps = {
  value: string
  position: number
  digitActive: boolean
  decimalPointActive: boolean
  onClickSegment: () => void
  onClickDecimalPoint: () => void
} & React.ButtonHTMLAttributes<HTMLButtonElement>

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

  return (
    <div className="flex flex-col gap-1">
      <div className="bg-foreground relative">
        <button
          onClick={onClickSegment}
          type="button"
          role="checkbox"
          className={cn("group h-12 w-8 bg-gray-900 px-0.5 py-1", className)}
          data-i={position}
          aria-checked={digitActive}
          aria-label={t("ConfigWizard.Device.SevenSegment.Digit", { position })}
          title={t("ConfigWizard.Device.SevenSegment.ToggleDigit", {
            position,
          })}
          {...props}
        >
          <SevenSegmentDigit
            value={value}
            digitActive={digitActive}
            dpActive={decimalPointActive}
          />
        </button>
        <button
          type="button"
          role="checkbox"
          onClick={onClickDecimalPoint}
          aria-checked={decimalPointActive}
          aria-label={t("ConfigWizard.Device.SevenSegment.DecimalPoint", {
            position,
          })}
          title={t("ConfigWizard.Device.SevenSegment.ToggleDecimalPoint", {
            position,
          })}
          className="group absolute right-0 bottom-0 pl-4 py-2"
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
      >
        {digitActive ? (
          <div className="bg-primary h-1 w-full"></div>
        ) : (
          <div className="bg-muted-foreground/50 h-1 w-full"></div>
        )}
      </button>
    </div>
  )
}
