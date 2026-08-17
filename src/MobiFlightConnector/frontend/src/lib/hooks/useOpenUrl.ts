import messageExchange from "@/lib/messageExchange"

const useOpenUrl = () => {
  const { publish } = messageExchange

  const openUrl = (url: string) => {
    publish({
      key: "CommandOpenLinkInBrowser",
      payload: { url: url },
    })
  }

  return openUrl
}

export default useOpenUrl
