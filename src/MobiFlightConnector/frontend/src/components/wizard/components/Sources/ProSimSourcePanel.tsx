import { Card, CardContent } from "@/components/ui/card"
import ProSimDataRefPanel from "@/components/wizard/components/InputActions/ProsimDataRefPanel"
import { ProSimSource, Source } from "@/types/config"

export interface ProSimSourcePanelProps {
  source: ProSimSource
  onSourceChange: (source: Source) => void
}
const ProSimSourcePanel = ({
  source,
  onSourceChange,
}: ProSimSourcePanelProps) => {

  return (
    <Card>
      <CardContent className="flex flex-col gap-4 pt-4">
        <ProSimDataRefPanel
          variant={"input"}
          selectedPath={source.ProSimDataRef.Path}
          onPresetChange={(prosimDataRef) => {
            // Handle the change here, for example, update the source object
            onSourceChange({
              ...source,
              ProSimDataRef: {
                Path: prosimDataRef.Name,
              },
            } as ProSimSource)
          }}
        />
      </CardContent>
    </Card>
  )
}
export default ProSimSourcePanel
