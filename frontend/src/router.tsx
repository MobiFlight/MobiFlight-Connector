import { createBrowserRouter } from "react-router-dom";
import * as Pages from "./pages/_index"
import App from "./App";

export const router = createBrowserRouter([
    { path: "/", element: <App />, children: [
        { index: true , element: <Pages.StartPage />, },
        { path: "settings", element: <Pages.SettingsPage />, },
        { path: "projects/:id", element: <Pages.ProjectPage />, },        
        { path: "*", element: <Pages.ErrorPage />}
    ]
    },
]);