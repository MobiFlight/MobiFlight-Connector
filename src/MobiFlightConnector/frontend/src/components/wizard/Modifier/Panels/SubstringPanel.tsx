import { Substring } from "@/types/modifier"
import { Badge } from "@/components/ui/badge"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"
import Input from "@/components/Input"
import { Trans, useTranslation } from "react-i18next"
import { Label } from "@/components/ui/label"

export const SubstringPanelTrigger = ({
  modifier,
}: {
  modifier: Substring
}) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-row items-center gap-2">
      <div className="text-md w-32 px-2 text-left font-semibold">
        {t("Dialog.Modifiers.Type.Substring.Label")}
      </div>
      <Trans
        i18nKey="Dialog.Modifiers.Type.Substring.Summary"
        values={{ start: modifier.Start, end: modifier.End }}
        components={{ badge: <Badge variant="secondary" /> }}
      />
    </div>
  )
}

export const SubstringPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Substring
  onChange: (updated: Substring) => void
}) => {
  const { t } = useTranslation()
  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Type.Substring.Description")}
      </div>
      <div className="flex flex-row items-center gap-4 pr-16 pb-4">
        <div className="flex flex-col gap-1">
          <Label htmlFor="start">
            {t("Dialog.Modifiers.Type.Substring.Start")}
          </Label>
          <Input
            id="start"
            value={modifier.Start}
            className="w-16"
            validateOnCommit={validateNumberInput}
            onChange={(value) => {
              onChange({ ...modifier, Start: value })
            }}
          />
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="end">
            {t("Dialog.Modifiers.Type.Substring.End")}
          </Label>
          <Input
            id="end"
            value={modifier.End}
            className="w-16"
            validateOnCommit={validateNumberInput}
            onChange={(value) => {
              onChange({ ...modifier, End: value })
            }}
          />
        </div>
      </div>
    </>
  )
}
