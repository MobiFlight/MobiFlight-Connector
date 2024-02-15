import { AppMessageKey } from "@/types";
import { AppMessage, FrontendMessageType } from "@/types/messages";
import { useEffect } from "react";

export const useMessageExchange = () => ({
  publish: (message: FrontendMessageType) => {
    window.chrome?.webview?.postMessage(message as any);
  },
});
// create a useAppMessage hook that listens for messages
// the paramaters are the AppMessageKey and the onReceiveMessage callback
// the callback is called when a message is received
export const useAppMessage = (
  key: AppMessageKey,
  onReceiveMessage: (message: any) => void
) => {
  const callback = onReceiveMessage;
  useEffect(() => {
    // add an event listener for the message key
    window.chrome?.webview?.addEventListener("message", (event: any) => {
      // if the message key matches the key, call the callback
      const appMessage = (JSON.parse(event.data) as AppMessage)
      if (appMessage.key === key) {
        console.log(`Received AppMessage -> ${appMessage.key}`);
        callback(appMessage);
      }
    });
    return () => {
      // remove the event listener when the component is unmounted
      window.chrome?.webview?.removeEventListener("message", onReceiveMessage);
    };
  }, [key]);
};
