import { useSearchParams } from "react-router"

export function useErrorFallbackTest() {
  const [searchParams] = useSearchParams()
  
  const trigger = (testid: string) => {
    if (
      searchParams.get("triggerError") === "true" &&
      searchParams.get("testid") === testid
    ) {
      throw new Error(`Test error triggered for testid: ${testid}`)
    }
  }

  return { trigger }
}
