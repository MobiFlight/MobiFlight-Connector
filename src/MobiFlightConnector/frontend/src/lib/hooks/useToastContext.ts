import { createContext, useContext } from "react"

export interface ToastContextType {
  toastId: string | number
  dismiss: () => void
}

export const ToastContext = createContext<ToastContextType | null>(null)

export const useToastContext = () => {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToastContext must be used within a ToastProvider')
  }
  return context
}