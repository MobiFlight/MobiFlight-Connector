import { Padding } from "@/types/modifier"
import { Label } from "@/components/ui/label"
import ComboBox from "@/components/ComboBox"
import { Trans, useTranslation } from "react-i18next"
import { Badge } from "@/components/ui/badge"
import Input from "@/components/Input"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"

export const PaddingPanelTrigger = ({ modifier }: { modifier: Padding }) => {
  const { t } = useTranslation("wizard")
  return (
    <div className="flex flex-row items-center gap-1">
      <div className="text-md w-32 px-2 text-left font-semibold">
        {t("ConfigWizard.Modifiers.Type.Padding.Label")}
      </div>
      <div className="flex flex-row items-center gap-2 text-sm">
        <Trans
          i18nKey="ConfigWizard.Modifiers.Type.Padding.Summary"
          ns="wizard"
          values={{
            length: modifier.Length,
            character:
              modifier.Character === " " ? "SPACE" : modifier.Character,
            direction: modifier.Direction,
          }}
          components={{
            badge: <Badge variant={"secondary"} />,
            kbd: <kbd />,
          }}
        />
      </div>
    </div>
  )
}

export const PaddingPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Padding
  onChange: (updated: Padding) => void
}) => {
  const { t } = useTranslation("wizard")
  const directionOptions = [
    { value: "Left", label: "Left" },
    { value: "Right", label: "Right" },
  ] as { value: "Left" | "Right"; label: string }[]

  const selectedDirection = directionOptions.find(
    (option) => option.value === modifier.Direction,
  )

  const setSelectedDirection = (
    item: { value: "Left" | "Right"; label: string } | null,
  ) => {
    if (item) {
      onChange({ ...modifier, Direction: item.value })
    }
  }
  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("ConfigWizard.Modifiers.Type.Padding.Description")}
      </div>
      <div className="flex flex-row items-center gap-4 pr-16 pb-4">
        <div className="flex flex-col gap-1">
          <Label htmlFor="length">
            {t("ConfigWizard.Modifiers.Type.Padding.Length")}
          </Label>
          <Input
            className="w-12"
            id="length"
            value={modifier.Length}
            validateOnCommit={validateNumberInput}
            onChange={(value) => onChange({ ...modifier, Length: value })}
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="character">
            {t("ConfigWizard.Modifiers.Type.Padding.Character")}
          </Label>
          <Input
            className="w-12"
            id="character"
            maxLength={1}
            value={modifier.Character}
            onChange={(value) => onChange({ ...modifier, Character: value })}
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="direction">
            {t("ConfigWizard.Modifiers.Type.Padding.Direction")}
          </Label>
          <ComboBox
            id="direction"
            items={directionOptions}
            selected={selectedDirection}
            getLabel={(item) => item.label}
            getValue={(item) => item.value}
            isSelected={(item) => item.value === selectedDirection?.value}
            setSelected={(item) => {
              setSelectedDirection(item ? item : null)
            }}
            searchPlaceholder={t(
              "ConfigWizard.Input.InputActions.Common.SearchPresets",
            )}
            variant="nofilter"
            widthClass="w-32"
          />
        </div>
      </div>
    </>
  )
}
