import React from "react"
import { Alert, AlertDescription, AlertTitle } from "./alert"
import { Button } from "./button"
import { ExternalToast } from "sonner"

export interface ToastProps {
  description?: React.ReactNode
  title?: React.ReactNode
  button?: {
    label: string
    onClick: () => void
    onCancel?: () => void
  }
  id: string | number,
  options? : ExternalToast
}

export const Toast = ({ description, title, button }: ToastProps) => {
  return (
    <Alert className="flex flex-col gap-1 p-4 pt-3 border-primary border-2 shadow-xl" onClick={button?.onCancel}>
      <AlertTitle className="text-md flex flex-row justify-between items-center">
        {title}
      </AlertTitle>
      <AlertDescription className="flex flex-row gap-4 items-center">
        <div className="text-md text-muted-foreground">{description}</div>
        <div className="flex flex-col gap-2">
        {button && (
        <Button
          onClick={button.onClick}
          className="mt-2 rounded bg-blue-500 px-3 py-1 text-sm text-white hover:bg-blue-600 h-8"
        >
          {button.label}
        </Button>
        )}
        </div>
      </AlertDescription>
      
    </Alert>
  )
}

export default Toast
