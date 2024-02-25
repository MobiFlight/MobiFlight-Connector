import { createBrowserRouter } from "react-router-dom";
import * as Pages from "./pages/_index"
import App from "./App";

export const router = createBrowserRouter([
    {
        path: "/", element: <App />, children: [
            { index: true, element: <Pages.StartPage /> },
            { path: "settings", element: <Pages.SettingsPage />, },
            { path: "log", element: <Pages.LogPage />, },
            { path: "test", element: <Pages.TestPage />, },
            { path: "devices", element: <Pages.DevicesPage />, },
            { path: "devices/:type/:id", element: <Pages.DeviceDetailPage />, children: [
                { path: "elements/:element", element: <Pages.DeviceElementDetailView />},
            ]},
            { path: "projects/:id", element: <Pages.ProjectPage />, },
            { path: "projects/:id/configs", element: <Pages.ConfigPage />, },
        ],
    },
    { path: "*", element: <Pages.ErrorPage /> },
]);