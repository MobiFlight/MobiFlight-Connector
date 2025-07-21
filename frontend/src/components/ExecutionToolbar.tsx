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
import { useTranslation } from "react-i18next"

export const ExecutionToolbar = () => {
  const { t } = useTranslation()
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
        content={settings?.AutoRun ? t("Project.Execution.AutoRun.Disable") : t("Project.Execution.AutoRun.Enable")}
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
          <div className="hidden lg:inline-flex pr-1">{t("Project.Execution.AutoRun.Label")}</div>
        </Button>
      </ToolTip>

      <ToolTip
        content={
          isRunning
            ? t("Project.Execution.Run.StopMode")
            : isTesting
              ? t("Project.Execution.Run.BlockedByTest")
              : t("Project.Execution.Run.Start")
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
              {!isRunning ? t("Project.Execution.Run.Label") : t("Project.Execution.Run.Stop")}
            </div>
          </Button>
        </div>
      </ToolTip>

      <ToolTip
        content={isTesting
              ? t("Project.Execution.Test.StopMode")
              : isRunning
                ? t("Project.Execution.Test.BlockedByRun")
                : t("Project.Execution.Test.Start")}>
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
                {!isTesting ? t("Project.Execution.Test.Label") : t("Project.Execution.Test.Stop")}
              </div>
            </Button>
          </div>
      </ToolTip>
    </div>
  )
}

export default ExecutionToolbar
