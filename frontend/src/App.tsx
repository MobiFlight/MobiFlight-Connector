import './App.css'
import { useTranslation } from "react-i18next"
import { publishOnMessageExchange, useAppMessage } from './lib/hooks/appMessage'
import { StatusBarUpdate } from './types'
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router'
import StartupProgress from './components/StartupProgress'

function App() {
  const [queryParameters] = useSearchParams()
  const { t } = useTranslation()

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
      setStartupProgress({ Value: 100, Text: "Finished!" })
    else
      setStartupProgress({ Value: value , Text: "Loading..." })
  }, [queryParameters])

  const { publish } = publishOnMessageExchange();
  const handleClick = () => {
    publish({ key: 'Test', payload: { 'Message': 'Hello World' } })
  }

  return (
    <>
      {startupProgress.Value < 100 ? (
        <StartupProgress
          value={startupProgress.Value}
          text={startupProgress.Text}
        />
      ) : (
        <><h1>MobiFlight 2025</h1>
          <h2>{t('app.greeting')}</h2>
          <p>Test123</p>
          <button onClick={handleClick}>Test</button>
        </>
      )}
    </>
  )
}

export default App
