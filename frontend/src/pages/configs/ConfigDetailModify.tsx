import { Button } from "@/components/ui/button"
import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card"
import { IConfigItem } from "@/types"
import { IconHelp } from "@tabler/icons-react"
import { ModifierContainer } from "./modifier/ModifierContainer"

interface ConfigDetailModifyViewProps {
  config: IConfigItem
  className?: string
  editMode: boolean
  onChange: (config: IConfigItem) => void
}

const ConfigDetailModifyView = (props: ConfigDetailModifyViewProps) => {
  const { config, editMode, className, onChange } = props

  return (
    <Card>
      <CardHeader className="">
        <h3 className="text-xl">Modifiers</h3>
        <p className="text-xs">Modify your event value (optional)</p>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        {config?.Modifiers.map((m, i) => <div key={i}><ModifierContainer modifier={m} /></div>)}
      </CardContent>
      <CardFooter className="flex flex-row items-center justify-between">
        <IconHelp>Learn more</IconHelp>
      </CardFooter>
    </Card>
  )
}

export default ConfigDetailModifyView
