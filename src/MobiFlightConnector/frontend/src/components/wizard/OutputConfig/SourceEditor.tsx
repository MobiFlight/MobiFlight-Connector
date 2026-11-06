import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { FsuipcOffsetPanel } from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"
import { VariablePanel } from "@/components/wizard/components/InputActions/VariablePanel"
import MsfsSourcePanel from "@/components/wizard/OutputConfig/Sources/MsfsSourcePanel"
import ProSimSourcePanel from "@/components/wizard/OutputConfig/Sources/ProSimSourcePanel"
import XplaneSourcePanel from "@/components/wizard/OutputConfig/Sources/XplaneSourcePanel"
import SourceTypeComboBox from "@/components/wizard/OutputConfig/SourceTypeComboBox"
import { SourceTypeOptions } from "@/lib/configWizard"
import {
  FsuipcSource,
  ProSimSource,
  SimConnectSource,
  Source,
  VariableSource,
  XplaneSource,
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
      <WizardSection>
        <SectionHeadline
          title={t("Dialog.ConfigWizard.Source.Title")}
          description={t("Dialog.ConfigWizard.Source.Description")}
        />
      </WizardSection>
      <WizardSection>
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
      </WizardSection>
      {selectedSourceType?.value === "SimConnectSource" && (
        <MsfsSourcePanel
          variant="details"
          source={source as SimConnectSource}
          onSourceChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "VariableSource" && (
        <VariablePanel
          variant="details"
          triggerType="output"
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
        <WizardSection>
          <SectionHeadline
            title={t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.Title")}
            description={t("Dialog.InputConfigWizard.InputActions.FsuipcOffset.Description")}
          />
          <FsuipcOffsetPanel
            fsuipc={(source as FsuipcSource).FSUIPC}
            onConfigChange={(fsuipc) => {
              onSourceChange({
                ...(source as FsuipcSource),
                FSUIPC: fsuipc,
              } as FsuipcSource)
            }}
          />
        </WizardSection>
      )}
      {selectedSourceType?.value === "ProSimSource" && (
        <ProSimSourcePanel
          source={source as ProSimSource}
          onSourceChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "XplaneSource" && (
        <XplaneSourcePanel
          variant="details"
          source={source as XplaneSource}
          onSourceChange={onSourceChange}
        />
      )}
    </div>
  )
}
export default SourceEditor
