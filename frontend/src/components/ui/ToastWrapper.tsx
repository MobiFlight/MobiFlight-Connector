import { toast as sonnerToast } from "sonner"
import Toast, { ToastProps } from "./Toast"
import { ToastProvider } from "./ToastContext"

export const toast = (props: ToastProps) => {
  const { button, options, onCancel, ...otherProps } = props

  return sonnerToast.custom(
    (id) => {
      const enhancedButton = button
        ? {
            ...button,
            onClick: () => {
              button.onClick()
              sonnerToast.dismiss(id)
            },
          }
        : undefined

      const enhancedCancel = onCancel
        ? () => {
            onCancel()
            sonnerToast.dismiss(id)
          }
        : undefined

      return (
        <ToastProvider toastId={id} dismiss={() => sonnerToast.dismiss(id)}>
          <Toast
            {...otherProps}
            button={enhancedButton}
            onCancel={enhancedCancel}
          />
        </ToastProvider>
      )
    },
    { ...options },
  )
}

export default toast
