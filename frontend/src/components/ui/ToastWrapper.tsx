import { toast as sonnerToast } from "sonner"
import Toast, { ToastProps } from "./Toast"

export const toast = (props: ToastProps) => {
  const { button, options } = props

  return sonnerToast.custom((id) => {
    const extendedButton = {
      ...button!,
      onClick: () => {
        button?.onClick()
        sonnerToast.dismiss(id)
      },

      onCancel: () => {
        button?.onCancel?.()
        sonnerToast.dismiss(id)
      },
    }
    return (<Toast {...props} button={extendedButton} />)}, { ...options })
}

export default toast
