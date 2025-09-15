import { Progress } from "./ui/progress"
import logo from "../assets/mobiflight-logo-border.png"

interface StartupProgressProps {
  value: number
  text: string
}

const StartupProgress = (props: StartupProgressProps) => {
  const { value, text } = props
  return (
    <>
      <div className="fixed inset-0 bg-linear-to-br from-indigo-500 from-10% via-sky-500 via-30% to-emerald-500 to-90% dark:from-indigo-500/10 dark:via-sky-500/0 dark:via-70% dark:to-emerald-500/5"></div>
      <div className="relative flex min-h-screen flex-col items-center justify-center gap-8 p-10">
        <div className="animate-low-bounce">
          <img
            src={logo}
            className="pointer-events-none h-36 w-36 shadow-xl shadow-slate-800/40"
          />
        </div>
        <div className="dark:h-10 rounded-full w-full max-w-xl dark:bg-linear-to-br dark:from-indigo-500 dark:from-10% dark:via-sky-500 dark:via-30% dark:to-emerald-500 dark:to-90% p-0.5">
          <Progress className="h-10 dark:h-9 max-w-xl dark:bg-black" value={value}></Progress>
        </div>
        <p className="text-white">{text}</p>
      </div>
    </>
  )
}

export default StartupProgress
