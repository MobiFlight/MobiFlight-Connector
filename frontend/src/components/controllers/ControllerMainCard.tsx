import ControllerCard from './ControllerCard'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Controller } from '@/types/controller'

const ControllerMainCard = () => {
  const controller: Controller[] = [
  ]
  return (
    <Card className="border-shadow-none border-none shadow-none">
        <CardHeader className="flex flex-row items-center justify-between">
          <div className="flex flex-col gap-2">
            <CardTitle>
              <h2>My Controllers</h2>
            </CardTitle>
            <CardDescription>
              Overview of my controllers used in my projects.
            </CardDescription>
          </div>
          <Button>Go to Settings</Button>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col gap-4">
            <div className="flex flex-row gap-2">
              <Button className="h-8 px-3 text-sm" variant={"default"}>
                All
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                MobiFlight
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                Joysticks
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                Midi
              </Button>
            </div>
            <div className="overflow-auto scroll-smooth">
              <div className="flex flex-row gap-6 pb-3">
                {controller.map((ctrl) => (
                  <ControllerCard
                    controller={ctrl}
                    key={ctrl.Name}
                    className="w-72"
                  />
                ))}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
  )
}

export default ControllerMainCard