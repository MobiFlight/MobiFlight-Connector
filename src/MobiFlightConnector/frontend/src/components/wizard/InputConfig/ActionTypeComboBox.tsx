import ComboBox from "@/components/ComboBox"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import { ActionTypeOptions } from "@/lib/configWizard"
import { useProjectStore } from "@/stores/projectStore"
import { Project } from "@/types"
import { useTranslation } from "react-i18next"

export type ActionTypeOption = {
  value: string
  isAvailable: (settings: Partial<Project>) => boolean
}

export type ActionTypeProps = {
  selectedActionType?: ActionTypeOption
  setSelectedActionType?: (option: ActionTypeOption | null) => void
}

const ActionTypeComboBox = ({
  selectedActionType,
  setSelectedActionType,
}: ActionTypeProps) => {
  const { t } = useTranslation()
  const { project } = useProjectStore()

  const filteredOptions = project
    ? ActionTypeOptions.filter((option) => option.isAvailable(project))
    : ActionTypeOptions

  return (
    <div className="flex flex-col gap-4">
      <SectionHeadline
        title={t("Dialog.InputConfigWizard.ActionType.Title")}
        description={t("Dialog.InputConfigWizard.ActionType.Description")}
      />
      <ComboBox
        data-testid="action-type-combobox"
        selected={selectedActionType}
        items={filteredOptions}
        getLabel={(item) =>
          t(
            `Dialog.InputConfigWizard.ActionType.Options.${item.value}.label`,
            item.value,
          )
        }
        getValue={(item) => item.value}
        isSelected={(item) => item.value === selectedActionType?.value}
        setSelected={(item) => {
          setSelectedActionType?.(item || null)
        }}
        widthClass="w-100"
      />
    </div>
  )
}
export default ActionTypeComboBox
