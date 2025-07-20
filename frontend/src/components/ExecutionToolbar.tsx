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
import ToolTip from "./ToolTip"

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
      <ToolTip
        content={settings?.AutoRun ? "Disable Auto Run" : "Enable Auto Run"}
      >
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
          <div className="hidden lg:inline-flex pr-1">Auto</div>
        </Button>
      </ToolTip>

      <ToolTip
        content={
          isRunning
            ? "Stop run mode"
            : isTesting
              ? "Test mode active. Stop it first."
              : "Start run mode and process all configs"
        }
      >
        <div className="inline-flex">
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
            <div className="hidden lg:inline-flex pr-1">
              {!isRunning ? "Run" : "Stop"}
            </div>
          </Button>
        </div>
      </ToolTip>

      <ToolTip
        content={isTesting
              ? "Stop test mode execution"
              : isRunning
                ? "Run mode active. Stop it first."
                : "Start test mode and test all configs"}>
          <div className="inline-flex">
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
              <div className="hidden lg:inline-flex pr-1">
                {!isTesting ? "Test" : "Stop"}
              </div>
            </Button>
          </div>
      </ToolTip>
    </div>
  )
}

export default ExecutionToolbar
