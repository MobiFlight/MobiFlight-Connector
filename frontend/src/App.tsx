import './App.css'
import { useTranslation } from "react-i18next"
import { publishOnMessageExchange, useAppMessage } from './lib/hooks/appMessage'
import { ILogMessage } from './types'

function App() {
  const { t } = useTranslation()
  
  useAppMessage('LogEntry', (log) => {
    const message = log.payload as ILogMessage
    console.table(message)
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
    </>
  )
}

export default App
