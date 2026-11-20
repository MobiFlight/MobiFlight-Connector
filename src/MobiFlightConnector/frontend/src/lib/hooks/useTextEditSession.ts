import { useRef, type FocusEvent } from "react"

export const isTextEditingTarget = (target: EventTarget | null): boolean => {
  if (!(target instanceof HTMLElement)) return false

  if (target.closest('[contenteditable="true"]')) return true

  if (target instanceof HTMLTextAreaElement) return true

  return (
    target instanceof HTMLInputElement &&
    ["text", "search", "number", "email", "password", "tel", "url"].includes(
      target.type,
    )
  )
}

const useTextEditSession = () => {
  const nextTextEditIdRef = useRef(0)
  const activeTextEditIdRef = useRef<number | null>(null)

  const getCurrentTextEditId = () => activeTextEditIdRef.current

  const handleFocusCapture = (event: FocusEvent<HTMLDivElement>) => {
    if (!isTextEditingTarget(event.target)) return

    nextTextEditIdRef.current += 1
    activeTextEditIdRef.current = nextTextEditIdRef.current
  }

  const handleBlurCapture = (event: FocusEvent<HTMLDivElement>) => {
    if (!isTextEditingTarget(event.target)) return

    activeTextEditIdRef.current = null
  }

  const resetTextEditSession = () => {
    activeTextEditIdRef.current = null
  }

  return {
    getCurrentTextEditId,
    handleFocusCapture,
    handleBlurCapture,
    resetTextEditSession,
  }
}

export default useTextEditSession
