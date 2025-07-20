import {
  IconFlask,
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
import IconAutoRun from "./icons/IconAutoRun"

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
    <div className="flex items-center gap-2" role="toolbar">
      <Button
        variant="ghost"
        className="text-md h-8 px-1 py-1 [&_svg]:size-6 gap-1"
        onClick={() => handleMenuItemClick({ action: "toggleAutoRun" })}
      >
        {settings?.AutoRun ? (
          <IconAutoRun className="stroke-yellow-500" />
        ) : (
          <IconAutoRun className="stroke-muted-foreground" />
        )}
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
          <>
          <IconPlayerStopFilled className="transition-scale transition-opacity fill-red-700 stroke-red-700" />
          Stop
          </>
        ) : (
          <>
          <IconPlayerStopFilled className="transition-scale transition-opacity fill-red-700 stroke-red-700" />
          <IconFlask className="stroke-sky-600" />
          Test
          </>
        )}
        
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
