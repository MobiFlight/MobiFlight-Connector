import {
  IconBulb,
  IconBulbFilled,
  IconPlayerPlay,
  IconPlayerPlayFilled,
  IconPlayerStop,
  IconPlayerStopFilled,
} from "@tabler/icons-react"
import { Button } from "./ui/button"
import { useExecutionStateStore } from "@/stores/executionStateStore"
import { useSettingsStore } from "@/stores/settingsStore"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { CommandProjectToolbarPayload } from "@/types/commands"
import { ExecutionState } from "@/types/messages"

export const ExecutionToolbar = () => {
  const { settings } = useSettingsStore()
  const { isRunning, isTesting, setIsRunning, setIsTesting } =
    useExecutionStateStore()
  const { publish } = publishOnMessageExchange()

  const handleMenuItemClick = (payload: CommandProjectToolbarPayload) => {
    publish({
      key: "CommandProjectToolbar",
      payload: payload,
    })
  }

  useAppMessage("ExecutionState", (message) => {
    console.log("ExecutionState message received", message.payload)
    const { IsRunning, IsTesting } = message.payload as ExecutionState
    setIsRunning(IsRunning)
    setIsTesting(IsTesting)
  })

  return (
    <div className="flex items-center gap-2">
      <Button
        variant="ghost"
        className="text-md h-8 px-1 py-1 [&_svg]:size-6 gap-1"
        onClick={() => handleMenuItemClick({ action: "toggleAutoRun" })}
      >
        {settings?.AutoRun ? (
          <IconBulbFilled className="stroke-yellow-500" />
        ) : (
          <IconBulb className="stroke-gray-500" />
        )}
        AutoStart
      </Button>
      <Button
        variant="ghost"
        className="text-md h-8 px-1 py-1 [&_svg]:size-6 gap-1"
        disabled={isRunning || isTesting}
        onClick={() => handleMenuItemClick({ action: "run" })}
      >
        {isRunning ? (
          <IconPlayerPlayFilled className="fill-green-600 stroke-green-600" />
        ) : (
          <IconPlayerPlay className="stroke-green-600" />
        )}
        Run
      </Button>
      <Button
        variant="ghost"
        className="text-md h-8 px-1 py-1 [&_svg]:size-6 gap-1"
        disabled={isRunning || isTesting}
        onClick={() => handleMenuItemClick({ action: "test" })}
      >
        {isTesting ? (
          <IconPlayerPlayFilled className="fill-sky-600 stroke-sky-600" />
        ) : (
          <IconPlayerPlay className="stroke-sky-600" />
        )}
        Test
      </Button>
      <Button
        variant="ghost"
        className="text-md h-8 px-1 py-1 [&_svg]:size-6 gap-1"
        disabled={!isRunning && !isTesting}
        onClick={() => handleMenuItemClick({ action: "stop" })}
      >
        {isRunning || isTesting ? (
          <IconPlayerStopFilled className="fill-red-700 stroke-red-700" />
        ) : (
          <IconPlayerStop className="stroke-muted-foreground" />
        )}
        <div className={isRunning || isTesting ? "" : "text-muted-foreground"}>
          Stop
        </div>
      </Button>
    </div>
  )
}

export default ExecutionToolbar
