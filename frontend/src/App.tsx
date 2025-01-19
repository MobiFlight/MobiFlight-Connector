import './App.css'
import { useTranslation } from "react-i18next"
import { publishOnMessageExchange, useAppMessage } from './lib/hooks/appMessage'
import { ILogMessage } from './types'
import { useState } from 'react'

function App() {
  const { t } = useTranslation()
  const [logEntries, setLogEntries] = useState<ILogMessage[]>([])

  useAppMessage('LogEntry', (log) => {
    const message = log.payload as ILogMessage
    setLogEntries((entries) => [...entries, message])
  })

  const { publish } = publishOnMessageExchange();
  const handleClick = () => {
    publish({ key: 'Test', payload: { 'Message': 'Hello World' } })
  }
  return (
    <>
      <h1>MobiFlight 2025</h1>
      <h2>{t('app.greeting')}</h2>
      <p>Test123</p>
      <button onClick={handleClick}>Test</button>
      <div>
        {logEntries.map((entry, index) => (
          <div key={index}>{entry.Message}</div>
        ))}
      </div>
    </>
  )
}

export default App
