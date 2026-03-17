import SplashLogo from "@/components/SplashLogo"
import { Progress } from "./ui/progress"
import { StatusBarUpdate } from "@/types/messages"
import { useEffect, useState } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { useNavigate, useSearchParams } from "react-router"
import { useTranslation } from "react-i18next"

const StartupProgress = () => {
  const { t } = useTranslation()
  // State for startup progress from app messages
  const [appStartupProgress, setAppStartupProgress] = useState<StatusBarUpdate>(
    {
      Value: 0,
      Text: "Startup.Starting",
    },
  )
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()
  useAppMessage("StatusBarUpdate", (message) => {
    setAppStartupProgress(message.payload as StatusBarUpdate)
  })
  const queryProgressValue = Number.parseInt(
    queryParameters.get("progress")?.toString() ?? "0",
  )
  const startupProgress =
    queryProgressValue > 0
      ? {
          Value: queryProgressValue,
          Text:
            queryProgressValue === 100
              ? "Startup.Finished"
              : "Startup.Test.Loading",
        }
      : appStartupProgress

  useEffect(() => {
    if (startupProgress.Value !== 100) return

    console.log("Finished loading, navigating to home")
    
    const timeoutId = setTimeout(() => {
      navigate("/home")
    }, 1000) // Add a small delay to allow users to see the completed progress bar

    return () => {
      clearTimeout(timeoutId)
    }
  }, [startupProgress.Value, navigate])

  return (
    <div className="relative flex min-h-screen min-w-lg flex-col items-center justify-center gap-8 p-10 lg:min-w-xl">
      <SplashLogo />
      <div className="w-full max-w-xl rounded-full p-0.5 dark:h-10 dark:bg-linear-to-br dark:from-indigo-500 dark:from-10% dark:via-sky-500 dark:via-30% dark:to-emerald-500 dark:to-90%">
        <Progress
          className="h-10 max-w-xl dark:h-9 dark:bg-black"
          value={startupProgress.Value}
        ></Progress>
      </div>
      <p className="text-white">{t(startupProgress.Text)}</p>
    </div>
  )
}

export default StartupProgress
