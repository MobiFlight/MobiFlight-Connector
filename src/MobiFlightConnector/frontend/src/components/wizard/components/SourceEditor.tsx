import { Card, CardContent } from "@/components/ui/card"
import SourceTypeComboBox, { SourceTypeOption } from "@/components/wizard/components/SourceTypeComboBox"
import { SourceTypeOptions } from "@/lib/configWizard"
import { Source } from "@/types/config"

export interface SourceEditorProps {
  source: Source | null
  onSourceChange: (selectedSourceType: SourceTypeOption | null) => void
}
const SourceEditor = ({
  source,
  onSourceChange,
}: SourceEditorProps) => {
  const selectedSourceType = source ? SourceTypeOptions.find((option) => option.value === source.Type) : undefined
  
  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <Card>
        <CardContent className="pt-4 flex flex-col gap-4">
          <div>Source Type: {source?.Type}</div>
        </CardContent>
      </Card>
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
        <CardContent className="pt-4">
          
        </CardContent>
      </Card>
    </div>
  )
}
export default SourceEditor
