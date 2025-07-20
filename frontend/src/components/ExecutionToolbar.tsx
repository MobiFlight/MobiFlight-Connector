import {
  IconFlask,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
} from "@tabler/icons-react"
import { Button } from "./ui/button"
import { useExecutionStateStore } from "@/stores/executionStateStore"
import { useSettingsStore } from "@/stores/settingsStore"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { CommandProjectToolbarPayload } from "@/types/commands"
import { ExecutionState } from "@/types/messages"
import IconAutoRun from "./icons/IconAutoRun"
import TwoStateIcon from "./icons/TwoStateIcon"

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
        disabled={isTesting}
        variant="ghost"
        className="text-md h-8 px-1 py-1 pr-2 [&_svg]:size-6 gap-1"
        onClick={() => handleMenuItemClick({ action: !isRunning ? "run" : "stop" })}
      >
        <TwoStateIcon 
          state={ isRunning }
          primaryIcon={IconPlayerPlayFilled}
          secondaryIcon={IconPlayerStopFilled}
          primaryClassName={ !isTesting ? "fill-green-600 stroke-green-600" : "fill-none stroke-2 stroke-muted-foreground" }
          secondaryClassName="fill-red-700 stroke-red-700"
        />
        {
          !isRunning ? "Run" : "Stop"
        }
      </Button>
      <Button
        disabled={isRunning}
        variant="ghost"
        className="text-md h-8 px-1 py-1 pr-2 [&_svg]:size-6 gap-1"
        onClick={() => handleMenuItemClick({ action: !isTesting ? "test" : "stop" })}
      >
        <TwoStateIcon 
          state={ isTesting }
          primaryIcon={IconFlask}
          secondaryIcon={IconPlayerStopFilled}
          primaryClassName="stroke-sky-600"
          secondaryClassName="fill-red-700 stroke-red-700"
        />
        {
          !isTesting ? "Test" : "Stop"
        }
      </Button>
    </div>
  )
}

export default ExecutionToolbar
