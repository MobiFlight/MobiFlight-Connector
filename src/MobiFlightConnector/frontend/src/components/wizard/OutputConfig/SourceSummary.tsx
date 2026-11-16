import { Label } from "@/components/ui/label"
import { Source } from "@/types/config"
import { FsuipcOffsetSummary } from "@/components/wizard/common/FsuipcOffsetPanel"
import MsfsPresetPanel from "@/components/wizard/common/MsfsPresetPanel"
import { Badge } from "@/components/ui/badge"
import {
  msfsSourceAdapter,
  proSimSourceAdapter,
  xplaneSourceAdapter,
} from "@/types/presets"
import XplanePresetPanel from "@/components/wizard/common/XplanePresetPanel"
import { useTranslation } from "react-i18next"
import { ProSimPresetPanel } from "@/components/wizard/common/ProSimPresetPanel"
import { VariablePanel } from "@/components/wizard/common/VariablePanel"
import SummaryButtons from "@/components/wizard/common/SummaryButtons"

export interface SourceSummaryProps {
  source: Source
  liveData: {
    rawValue: string | null | undefined
    finalValue: string | null | undefined
  }
  onEdit: () => void
  onRemove: () => void
}

const SourceSummary = ({
  source,
  liveData,
  onEdit,
  onRemove,
}: SourceSummaryProps) => {
  const { t } = useTranslation("wizard")
  return (
    <div
      className="group hover:bg-accent/30 flex flex-row items-center justify-between gap-4 rounded-md p-2"
      onDoubleClick={onEdit}
    >
      <div className="flex grow flex-row items-end">
        <div className="flex w-1/4 flex-col gap-1">
          <Label>{t("ConfigWizard.Source.Label")}</Label>
          <Badge variant={"secondary"} className="w-fit">
            {t(`ConfigWizard.SourceType.Options.${source.Type}.label`)}
          </Badge>
        </div>

        <div className="flex w-1/4 flex-col gap-1">
          <Label>{t("ConfigList.Header.RawValue")}</Label>
          <Badge variant={"secondary"} className="w-fit">
            {liveData.rawValue ?? "-"}
          </Badge>
        </div>

        <div className="flex w-1/2 flex-col">
          {source.Type === "SimConnectSource" && (
            <MsfsPresetPanel
              variant="summary"
              adapter={msfsSourceAdapter}
              config={source}
            />
          )}

          {source.Type === "XplaneSource" && (
            <XplanePresetPanel
              variant="summary"
              adapter={xplaneSourceAdapter}
              config={source}
            />
          )}

          {source.Type === "VariableSource" && (
            <VariablePanel
              variant="summary"
              mode="OutputConfigItem"
              currentVariable={source.MobiFlightVariable}
              onVariableChange={() => {}}
            />
          )}

          {source.Type === "FsuipcSource" && (
            <div className="flex grow flex-row items-center justify-between">
              <FsuipcOffsetSummary
                mode="OutputConfigItem"
                config={source.FSUIPC}
              />
            </div>
          )}

          {source.Type === "ProSimSource" && (
            <ProSimPresetPanel
              variant="summary"
              adapter={proSimSourceAdapter}
              config={source}
            />
          )}
        </div>
      </div>

      <SummaryButtons
        onEdit={onEdit}
        editLabel={t("ConfigWizard.Source.Edit")}
        onRemove={onRemove}
        removeLabel={t("ConfigWizard.Source.Remove")}
      />
    </div>
  )
}

export default SourceSummary
