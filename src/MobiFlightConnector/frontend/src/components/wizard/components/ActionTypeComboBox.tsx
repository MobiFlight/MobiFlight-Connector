import ComboBox from "@/components/ComboBox"
import { useTranslation } from "react-i18next"

export type ActionTypeOption = {
  value: string
  label: string
}

export const ActionTypeOptions: ActionTypeOption[] = [
  { value: "MSFS2020CustomInputAction", label: "MSFS2020CustomInputAction" },
  { value: "XplaneInputAction", label: "XplaneInputAction" },
  { value: "ProSimInputAction", label: "ProSimInputAction" },
  { value: "VariableInputAction", label: "VariableInputAction" },
  { value: "RetriggerInputAction", label: "RetriggerInputAction" },
  { value: "KeyInputAction", label: "KeyInputAction" },
  { value: "VJoyInputAction", label: "VJoyInputAction" },
  { value: "FsuipcOffsetInputAction", label: "FsuipcOffsetInputAction" },
  { value: "PmdgEventIdInputAction", label: "PmdgEventIdInputAction" },
  { value: "LuaMacroInputAction", label: "LuaMacroInputAction" },
  { value: "JeehellInputAction", label: "JeehellInputAction" },
  { value: "EventIdInputAction", label: "EventIdInputAction" },
]

export type ActionTypeProps = {
  selectedActionType?: ActionTypeOption
  setSelectedActionType?: (option: ActionTypeOption | undefined) => void
}

const ActionTypeComboBox = ({
  selectedActionType,
  setSelectedActionType,
}: ActionTypeProps) => {
  const { t } = useTranslation()
  console.log("Selected Action Type:", selectedActionType)
  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">{t("Dialog.InputConfigWizard.ActionType.Title")}</div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.InputConfigWizard.ActionType.Description")}
        </div>
      </div>
      <ComboBox
        selected={selectedActionType}
        items={ActionTypeOptions}
        getLabel={(item) => t(`Dialog.InputConfigWizard.ActionType.Options.${item.value}`, item.value)}
        getValue={(item) => item.value}
        isSelected={(item) => item.value === selectedActionType?.value}
        setSelected={(item) => {
          setSelectedActionType?.(item || undefined)
        }}
        widthClass="w-100"
      />
    </div>
  )
}
export default ActionTypeComboBox
