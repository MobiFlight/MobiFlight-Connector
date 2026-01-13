import { useLocation, useNavigate } from "react-router"

export type ModalOptions = {
  route: string
}

export function useModal() {
  const navigate = useNavigate()
  const location = useLocation()

  const showOverlay = (options: ModalOptions) => {
    const route = options.route
    navigate(route, { state: { backgroundLocation: location, ...options } })
  }

  const showStandalone = (options: ModalOptions) => {
    const route = options.route
    navigate(route, { state: { ...options } })
  }

  return { showOverlay, showStandalone }
}