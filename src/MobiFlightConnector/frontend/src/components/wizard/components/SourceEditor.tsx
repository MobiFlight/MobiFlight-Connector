import { Card, CardContent } from "@/components/ui/card"
import { FsuipcOffsetPanel } from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"
import { VariablePanel } from "@/components/wizard/components/InputActions/VariablePanel"
import MsfsSourcePanel from "@/components/wizard/components/Sources/MsfsSourcePanel"
import ProSimSourcePanel from "@/components/wizard/components/Sources/ProSimSourcePanel"
import SourceTypeComboBox from "@/components/wizard/components/SourceTypeComboBox"
import { SourceTypeOptions } from "@/lib/configWizard"
import {
  FsuipcSource,
  ProSimSource,
  SimConnectSource,
  Source,
  VariableSource,
} from "@/types/config"
import { SourceDefaults } from "@/types/defaults"
import { useTranslation } from "react-i18next"

export interface SourceEditorProps {
  source: Source | null
  onSourceChange: (source: Source | null) => void
}
const SourceEditor = ({ source, onSourceChange }: SourceEditorProps) => {
  const { t } = useTranslation()
  const selectedSourceType = source
    ? SourceTypeOptions.find((option) => option.value === source.Type)
    : undefined

  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">
          {t("Dialog.ConfigWizard.Source.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.ConfigWizard.Source.Description")}
        </div>
      </div>
      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-col gap-4">
            <div className="flex flex-row items-end justify-between">
              <SourceTypeComboBox
                selectedSourceType={selectedSourceType}
                setSelectedSourceType={(option) => {
                  switch (option?.value) {
                    case "SimConnectSource":
                      onSourceChange(SourceDefaults.createSimConnectSource())
                      break
                    case "FsuipcSource":
                      onSourceChange(SourceDefaults.createFsuipcSource())
                      break
                    case "ProSimSource":
                      onSourceChange(SourceDefaults.createProSimSource())
                      break
                    case "VariableSource":
                      onSourceChange(SourceDefaults.createVariableSource())
                      break
                    case "XplaneSource":
                      onSourceChange(SourceDefaults.createXplaneSource())
                      break
                    default:
                      onSourceChange(null)
                  }
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      {selectedSourceType?.value === "SimConnectSource" && (
        <MsfsSourcePanel
          source={source as SimConnectSource}
          onSourceChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "VariableSource" && (
        <VariablePanel
          variant="details"
          currentVariable={(source as VariableSource).MobiFlightVariable}
          onVariableChange={(variable) => {
            onSourceChange({
              ...source,
              MobiFlightVariable: variable,
            } as VariableSource)
          }}
        />
      )}
      {selectedSourceType?.value === "FsuipcSource" && (
        <Card>
          <CardContent className="flex flex-col gap-4 pt-4">
            <div className="flex flex-col">
              <div className="text-lg font-semibold">
                {t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.Title")}
              </div>
              <div className="text-muted-foreground text-sm">
                {t(
                  "Dialog.InputConfigWizard.InputActions.FsuipcOffset.Description",
                )}
              </div>
            </div>
            <FsuipcOffsetPanel
              fsuipc={(source as FsuipcSource).FSUIPC}
              onConfigChange={(fsuipc) => {
                onSourceChange({
                  ...(source as FsuipcSource),
                  FSUIPC: fsuipc,
                } as FsuipcSource)
              }}
            />
          </CardContent>
        </Card>
      )}
      {selectedSourceType?.value === "ProSimSource" && (
        <ProSimSourcePanel
          source={source as ProSimSource}
          onSourceChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "XplaneSource" && (
        <div>Xplane options</div>
      )}
    </div>
  )
}
export default SourceEditor
