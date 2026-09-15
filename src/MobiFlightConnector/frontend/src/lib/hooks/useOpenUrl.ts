import messageExchange from "@/lib/messageExchange"

const useOpenUrl = () => {
  const { publish } = messageExchange

  const openUrl = (url: string) => {
    publish({
      key: "CommandOpenLinkInBrowser",
      payload: { url: url },
    })

    if (url.startsWith("mailto:")) {
      window.location.href = url
    } else if (!window.__MOBIFLIGHT__?.wsUrl) {
      window.open(url, "_blank", "noopener,noreferrer")
    }
  }

  return openUrl
}

export default useOpenUrl
