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
    }
}