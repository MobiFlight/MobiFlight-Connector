import { IconBulb, IconPlayerPlay, IconPlayerStop } from "@tabler/icons-react"
import { Button } from "./ui/button"

export const ExecutionToolbar = () => {
  return (
    <div className="flex items-center gap-2">
      <Button variant="ghost" className="px-2 py-1 text-md h-8 [&_svg]:size-6">
        <IconBulb className="stroke-gray-500" />
        AutoStart
      </Button>
      <Button variant="ghost" className="px-2 py-1 text-md h-8 [&_svg]:size-6">
        <IconPlayerPlay className="stroke-green-600" />
        Run
      </Button>
      <Button variant="ghost" className="px-2 py-1 text-md h-8 [&_svg]:size-6">
        <IconPlayerPlay className="stroke-sky-600" />
        Test
      </Button>
      <Button variant="ghost" className="px-2 py-1 text-md h-8 [&_svg]:size-6">
        <IconPlayerStop className="stroke-red-700" />
        Stop
      </Button>
    </div>
  )
}

export default ExecutionToolbar
