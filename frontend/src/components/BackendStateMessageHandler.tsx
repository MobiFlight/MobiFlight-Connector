import { ReactNode } from "react"
import { useBackendStateAppMessages } from "@/lib/hooks/useBackendStateAppMessages"

type BackendStateMessageHandlerProps = {
  children: ReactNode
}

export function BackendStateMessageHandler({
  children,
}: BackendStateMessageHandlerProps) {
  useBackendStateAppMessages()
  return <>{children}</>
}
