import React from 'react'
import ReactDOM from 'react-dom/client'
import { RouterProvider } from "react-router-dom"
import './index.css'
import './i18n';

import { router } from "./router"
import { ThemeProvider } from './components/ui/theme-provider';


ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
    <React.Suspense fallback='Loading'>
      <RouterProvider router={router} />
    </React.Suspense>
    </ThemeProvider>
  </React.StrictMode>
);
