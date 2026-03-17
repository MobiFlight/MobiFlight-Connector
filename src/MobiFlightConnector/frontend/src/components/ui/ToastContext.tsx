import { ToastContext } from "@/lib/hooks/useToastContext"

export const ToastProvider = ({ 
  children, 
  toastId, 
  dismiss 
}: { 
  children: React.ReactNode
  toastId: string | number
  dismiss: () => void
}) => {
  return (
    <ToastContext.Provider value={{ toastId, dismiss }}>
      {children}
    </ToastContext.Provider>
  )
}