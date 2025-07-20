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
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "./ui/tooltip"

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
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            <Button
              variant="ghost"
              className="text-md h-8 gap-1 p-1 [&_svg]:size-6"
              onClick={() => handleMenuItemClick({ action: "toggleAutoRun" })}
            >
              <IconAutoRun
                className={
                  settings?.AutoRun
                    ? "stroke-yellow-500 transition-colors"
                    : "stroke-muted-foreground transition-colors"
                }
              />
              <div className="hidden lg:inline-flex">Auto</div>
            </Button>
          </TooltipTrigger>
          <TooltipContent>
            <p>{settings?.AutoRun ? "Disable Auto Run" : "Enable Auto Run"}</p>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>

      <Button
        disabled={isTesting}
        variant="ghost"
        className="text-md h-8 gap-1 p-1 [&_svg]:size-6"
        onClick={() =>
          handleMenuItemClick({ action: !isRunning ? "run" : "stop" })
        }
      >
        <TwoStateIcon
          state={isRunning}
          primaryIcon={IconPlayerPlayFilled}
          secondaryIcon={IconPlayerStopFilled}
          primaryClassName={
            !isTesting
              ? "fill-green-600 stroke-green-600"
              : "fill-none stroke-2 stroke-muted-foreground"
          }
          secondaryClassName="fill-red-700 stroke-red-700"
        />
        <div className="hidden lg:inline-flex">{!isRunning ? "Run" : "Stop"}</div>
      </Button>
      <Button
        disabled={isRunning}
        variant="ghost"
        className="text-md h-8 gap-1 p-1 [&_svg]:size-6"
        onClick={() =>
          handleMenuItemClick({ action: !isTesting ? "test" : "stop" })
        }
      >
        <TwoStateIcon
          state={isTesting}
          primaryIcon={IconFlask}
          secondaryIcon={IconPlayerStopFilled}
          primaryClassName={
            !isRunning
              ? "stroke-sky-600"
              : "fill-none stroke-2 stroke-muted-foreground"
          }
          secondaryClassName="fill-red-700 stroke-red-700"
        />
        <div className="hidden lg:inline-flex">{!isTesting ? "Test" : "Stop"}</div>
      </Button>
    </div>
  )
}

export default ExecutionToolbar
