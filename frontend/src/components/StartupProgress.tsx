import SplashLogo from "@/components/SplashLogo"
import { Progress } from "./ui/progress"
import { StatusBarUpdate } from "@/types/messages"
import { useEffect, useState } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { useNavigate, useSearchParams } from "react-router"

const StartupProgress = () => {
  // State for startup progress from app messages
  const [appStartupProgress, setAppStartupProgress] = useState<StatusBarUpdate>(
    {
      Value: 0,
      Text: "Starting...",
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
            queryProgressValue === 100 ? "Loading complete..." : "Loading...",
        }
      : appStartupProgress

  useEffect(() => {
    if (startupProgress.Value == 100) {
      console.log("Finished loading, navigating to home")
      setTimeout(() => {
        navigate("/home")
      }, 1000) // Add a small delay to allow users to see the completed progress bar
    }
  }, [startupProgress.Value, navigate])

  return (
    <div className="relative min-w-lg lg:min-w-xl flex min-h-screen flex-col items-center justify-center gap-8 p-10">
      <SplashLogo />
      <div className="w-full max-w-xl rounded-full p-0.5 dark:h-10 dark:bg-linear-to-br dark:from-indigo-500 dark:from-10% dark:via-sky-500 dark:via-30% dark:to-emerald-500 dark:to-90%">
        <Progress
          className="h-10 max-w-xl dark:h-9 dark:bg-black"
          value={startupProgress.Value}
        ></Progress>
      </div>
      <p className="text-white">{startupProgress.Text}</p>
    </div>
  )
}

export default StartupProgress
