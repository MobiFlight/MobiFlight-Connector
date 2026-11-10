import ComboBox from "@/components/ComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
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
  const { t } = useTranslation("wizard")
  const { project } = useProjectStore()

  const filteredOptions = project
    ? SourceTypeOptions.filter((option) => option.isAvailable(project))
    : SourceTypeOptions

  return (
    <div className="flex flex-col gap-4">
      <SectionHeadline
        title={t("ConfigWizard.SourceType.Title")}
        description={t("ConfigWizard.SourceType.Description")}
      />
      <ComboBox
        data-testid="source-type-combobox"
        selected={selectedSourceType}
        items={filteredOptions}
        getLabel={(item) =>
          t(
            `ConfigWizard.SourceType.Options.${item.value}.label`
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
