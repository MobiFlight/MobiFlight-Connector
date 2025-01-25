import { useAppMessage } from '@/lib/hooks/appMessage'
import { StatusBarUpdate } from '@/types'
import { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router'
import StartupProgress from '@/components/StartupProgress'

function StartupPage() {
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()

  const [startupProgress, setStartupProgress] = useState<StatusBarUpdate>(
    { Value: 0, Text: "Starting..." },
  )

  useAppMessage("StatusBarUpdate", (message) => {
    setStartupProgress(message.payload as StatusBarUpdate)
  })

  // this allows to get beyond the startup screen
  // by setting the progress to 100 via url parameter
  useEffect(() => {
    // convert string to number    
    const value = Number.parseInt(queryParameters.get("progress")?.toString() ?? "0")
    if (value == 100)
      navigate("/config")
    else
      setStartupProgress({ Value: value, Text: "Loading..." })
  }, [navigate, queryParameters])

  return (
    <StartupProgress
      value={startupProgress.Value}
      text={startupProgress.Text}
    />
  )
}

export default StartupPage
