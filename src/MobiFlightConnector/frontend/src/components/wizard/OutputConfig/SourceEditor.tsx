import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { FsuipcOffsetPanel } from "@/components/wizard/common/FsuipcOffsetPanel"
import { VariablePanel } from "@/components/wizard/common/VariablePanel"
import MsfsPresetPanel from "@/components/wizard/common/MsfsPresetPanel"
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
import {
  msfsSourceAdapter,
  proSimSourceAdapter,
  xplaneSourceAdapter,
} from "@/types/presets"
import XplanePresetPanel from "@/components/wizard/common/XplanePresetPanel"
import DrawerBreadcrumb from "@/components/wizard/common/DrawerTitle"
import { ProSimPresetPanel } from "@/components/wizard/common/ProSimPresetPanel"

export interface SourceEditorProps {
  configName: string
  source: Source | null
  onSourceChange: (source: Source | null) => void
}
const SourceEditor = ({
  configName,
  source,
  onSourceChange,
}: SourceEditorProps) => {
  const { t } = useTranslation("wizard")
  const selectedSourceType = source
    ? SourceTypeOptions.find((option) => option.value === source.Type)
    : undefined

  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <SectionHeadline
        title={
          <DrawerBreadcrumb
            mode={"OutputConfigItem"}
            crumbs={[
              configName,
              t(`Dialog.InputConfigWizard.ConfigTrigger.Title`),
              t("ConfigWizard.Source.Title"),
            ]}
          />
        }
        description={t("ConfigWizard.Source.Description")}
      />
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
        <MsfsPresetPanel
          variant="details"
          adapter={msfsSourceAdapter}
          config={source as SimConnectSource}
          onConfigChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "XplaneSource" && (
        <XplanePresetPanel
          variant="details"
          adapter={xplaneSourceAdapter}
          config={source as XplaneSource}
          onConfigChange={onSourceChange}
        />
      )}
      {selectedSourceType?.value === "VariableSource" && (
        <VariablePanel
          variant="details"
          mode="OutputConfigItem"
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
            title={t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.Title",
            )}
            description={t(
              "Dialog.InputConfigWizard.InputActions.FsuipcOffset.Description",
            )}
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
        <ProSimPresetPanel
          variant="details"
          adapter={proSimSourceAdapter}
          config={source as ProSimSource}
          onConfigChange={onSourceChange}
        />
      )}
    </div>
  )
}
export default SourceEditor
