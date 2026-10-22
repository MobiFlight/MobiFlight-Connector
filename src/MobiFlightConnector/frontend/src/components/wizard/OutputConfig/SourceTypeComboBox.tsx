import ComboBox from "@/components/ComboBox"
import { SourceTypeOptions } from "@/lib/configWizard"
import { useProjectStore } from "@/stores/projectStore"
import { Project } from "@/types"
import type { SourceType } from "@/types/config"
import { useTranslation } from "react-i18next"

export type SourceTypeOption = {
  value: SourceType
  isAvailable: (settings: Partial<Project>) => boolean
}

export type SourceTypeProps = {
  selectedSourceType?: SourceTypeOption
  setSelectedSourceType?: (option: SourceTypeOption | null) => void
}

const SourceTypeComboBox = ({
  selectedSourceType,
  setSelectedSourceType,
}: SourceTypeProps) => {
  const { t } = useTranslation()
  const { project } = useProjectStore()

  const filteredOptions = project
    ? SourceTypeOptions.filter((option) => option.isAvailable(project))
    : SourceTypeOptions

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          {t("Dialog.ConfigWizard.SourceType.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.ConfigWizard.SourceType.Description")}
        </div>
      </div>
      <ComboBox
        data-testid="source-type-combobox"
        selected={selectedSourceType}
        items={filteredOptions}
        getLabel={(item) =>
          t(
            `Dialog.ConfigWizard.SourceType.Options.${item.value}.label`
          )
        }
        getValue={(item) => item.value}
        isSelected={(item) => item.value === selectedSourceType?.value}
        setSelected={(item) => {
          setSelectedSourceType?.(item || null)
        }}
        align="start"
        widthClass="w-70"
      />
    </div>
  )
}
export default SourceTypeComboBox
