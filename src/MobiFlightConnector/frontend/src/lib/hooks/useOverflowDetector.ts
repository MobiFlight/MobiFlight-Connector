import { RefObject, useCallback, useEffect, useState } from "react"

export const useOverflowDetector = (ref: RefObject<HTMLElement | null>) => {
  const [overflow, setOverflow] = useState({ left: false, right: false, any: false })

  const checkOverflow = useCallback(() => {
    const element = ref.current
    if (!element) return
    const isOverflowingLeft = element.scrollLeft > 0
    const isOverflowingRight =
      (element.scrollLeft + element.clientWidth) < element.scrollWidth

    setOverflow({
      left: isOverflowingLeft,
      right: isOverflowingRight,
      any: isOverflowingLeft || isOverflowingRight,
    })
  }, [ref])

  useEffect(() => {
    const element = ref.current
    if (!element) return

    checkOverflow()
    const resizeObserver = new ResizeObserver(() => {
      checkOverflow()
    })

    resizeObserver.observe(element)
    element.addEventListener("scroll", checkOverflow)
    window.addEventListener("resize", checkOverflow)

    return () => {
      element.removeEventListener("scroll", checkOverflow)
      window.removeEventListener("resize", checkOverflow)
    }
  }, [ref, checkOverflow])

  return { overflow, checkOverflow }
}
