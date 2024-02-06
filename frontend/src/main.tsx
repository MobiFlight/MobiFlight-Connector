import React from 'react'
import ReactDOM from 'react-dom/client'
import { RouterProvider } from "react-router-dom"
import './index.css'
import './i18n';

import { router } from "./router"


ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <React.Suspense fallback='Loading'>
      <RouterProvider router={router} />
    </React.Suspense>
  </React.StrictMode>
);
