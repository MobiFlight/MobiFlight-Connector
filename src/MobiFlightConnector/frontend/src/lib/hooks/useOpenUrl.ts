import useMessageExchange from "@/lib/hooks/useMessageExchange"

const useOpenUrl = () => {
  const { publish } = useMessageExchange()

  const openUrl = (url: string) => {
    publish({
      key: "CommandOpenLinkInBrowser",
      payload: { url: url },
    })
  }

  return openUrl
}

export default useOpenUrl
