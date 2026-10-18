import { Card, CardContent } from "@/components/ui/card"
import SourceTypeComboBox, {
  SourceTypeOption,
} from "@/components/wizard/components/SourceTypeComboBox"
import { SourceTypeOptions } from "@/lib/configWizard"
import { Source } from "@/types/config"
import { useTranslation } from "react-i18next"

export interface SourceEditorProps {
  source: Source | null
  onSourceChange: (selectedSourceType: SourceTypeOption | null) => void
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
                  onSourceChange(option)
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent className="pt-4"></CardContent>
      </Card>
    </div>
  )
}
export default SourceEditor
