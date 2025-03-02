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
      <div className="fixed inset-0 bg-gradient-to-br from-indigo-500 from-10% via-sky-500 via-30% to-emerald-500 to-90%"></div>
      <div className="relative flex min-h-screen flex-col items-center justify-center gap-8 p-10">
        <div className="animate-low-bounce">
          <img
            src={logo}
            className="pointer-events-none h-36 w-36 shadow-xl shadow-slate-800/40"
          />
        </div>
        <Progress className="h-10 max-w-xl" value={value}></Progress>
        <p className="text-white">{text}</p>
      </div>
    </>
  )
}

export default StartupProgress
