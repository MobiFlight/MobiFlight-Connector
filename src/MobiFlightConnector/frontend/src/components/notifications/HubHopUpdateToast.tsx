import { useHubHopState } from "@/stores/stateStore"
import { Progress } from "../ui/progress"
import {
  IconCircleCheckFilled,
  IconCircleXFilled,
  IconLoader2,
} from "@tabler/icons-react"
import { useEffect } from "react"
import { useTranslation } from "react-i18next"
import { useToastContext } from "@/lib/hooks/useToastContext"

export type HubHopUpdateToastProps = {
  timeout?: number
}

const HubHopUpdateToast = ({ timeout = 2000 }: HubHopUpdateToastProps) => {
  const HubHopState = useHubHopState()
  const { t } = useTranslation()
  const { dismiss } = useToastContext()

  useEffect(() => {
    if (HubHopState?.Result === "Success" || HubHopState?.Result === "Error") {
      // Auto-dismiss the toast after completion
      const timer = setTimeout(() => {
        dismiss()
      }, timeout) // Dismiss after specified timeout
      return () => clearTimeout(timer)
    }
  }, [HubHopState, dismiss, timeout])

  return HubHopState?.Result === "InProgress" ? (
    <div className="flex flex-row items-center gap-2">
      <IconLoader2 className="animate-spin" />
      <Progress value={HubHopState?.UpdateProgress} className="h-6 w-full" />
    </div>
  ) : HubHopState?.Result === "Success" ? (
    <div className="flex flex-row items-center gap-2">
      <IconCircleCheckFilled className="fill-green-700" />
      {t("General.HubHopUpdate.Success")}
    </div>
  ) : (
    <div className="flex flex-row items-center gap-2">
      <IconCircleXFilled className="fill-red-700" />
      <div>{t("General.HubHopUpdate.Error")}</div>
    </div>
  )
}

export default HubHopUpdateToast
