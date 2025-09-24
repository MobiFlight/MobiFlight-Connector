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
  }
  id: string | number,
  options? : ExternalToast
}

export const Toast = ({ description, title, button }: ToastProps) => {
  return (
    <Alert className="p-6 border-primary border-2 shadow-xl">
      <AlertTitle className="text-lg">{title}</AlertTitle>
      <AlertDescription className="flex flex-row gap-4 items-center">
        <div className="text-md text-muted-foreground">{description}</div>
        {button && (
        <Button
          onClick={button.onClick}
          className="mt-2 rounded bg-blue-500 px-3 py-1 text-sm text-white hover:bg-blue-600"
        >
          {button.label}
        </Button>
      )}
      </AlertDescription>
      
    </Alert>
  )
}

export default Toast
