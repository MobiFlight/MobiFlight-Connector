import './App.css'
import { useTranslation } from "react-i18next"

function App() {
  const { t } = useTranslation()

  return (
    <>
      <h1>MobiFlight 2025</h1>
      <h2>{t('app.greeting')}</h2>
      <p>Test1</p>
    </>
  )
}

export default App
