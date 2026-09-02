export {};

declare global {
    interface Window {
        chrome?: {
            webview?: {
                addEventListener: (
                    message: string,
                    handler: (message: Event) => void, // not sure yet what the event type should be
                ) => void;
                removeEventListener: (
                    message: string,
                    handler: (message: Event) => void, // not sure yet what the event type should be
                ) => void;
                postMessage: (message: object) => void;
            };
        };
        __MOBIFLIGHT__?: {
            // WebSocket server URL injected by the .NET host before navigation, e.g.
            // "ws://127.0.0.1:8321/" - see FrontendPanel.InitializeWebView and
            // docs/architecture/frontend-backend-messaging.md. Absent in a plain browser tab
            // (falls back to VITE_MF_WS_URL) and on the auth WebView, which stays on postMessage.
            wsUrl?: string;
        };
    }
}