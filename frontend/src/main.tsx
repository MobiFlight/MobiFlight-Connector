import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import './i18n.ts'
import App from './App.tsx'
import { BrowserRouter, Route, Routes } from 'react-router'
import ConfigPage from './pages/ConfigList'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />} />
        <Route path="/config" element={<App />}>
          <Route index element={<ConfigPage />} />
        </Route>
        <Route index path="/index.html" element={<App />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)