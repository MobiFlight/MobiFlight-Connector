import { createBrowserRouter } from "react-router-dom";
import * as Page from "./pages/_index"
import App from "./App";

export const router = createBrowserRouter([
    { path: "/", element: <App />, children: [
        { index: true , element: <Page.Start />, },
        { path: "settings", element: <Page.Settings />, },
        { path: "projects/:id", element: <Page.Project />, },
        { path: "*", element: <Page.Error />}
    ]
    },
]);