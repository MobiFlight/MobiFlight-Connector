import { describe, it, expect, beforeEach, afterEach, vi } from "vitest"
import { CommandOpenLinkInBrowser } from "@/types/commands"

const testCommand: CommandOpenLinkInBrowser = {
  key: "CommandOpenLinkInBrowser",
  payload: { url: "https://example.test" },
}

class FakeWebSocket {
  static readonly OPEN = 1
  static readonly CONNECTING = 0
  static instances: FakeWebSocket[] = []

  readyState = FakeWebSocket.CONNECTING
  onopen: (() => void) | null = null
  onmessage: ((event: { data: string }) => void) | null = null
  onclose: (() => void) | null = null
  onerror: ((event: unknown) => void) | null = null
  sent: string[] = []

  constructor(public url: string) {
    FakeWebSocket.instances.push(this)
  }

  send(data: string) {
    this.sent.push(data)
  }

  triggerOpen() {
    this.readyState = FakeWebSocket.OPEN
    this.onopen?.()
  }

  triggerClose() {
    this.onclose?.()
  }
}

describe("messageClient", () => {
  beforeEach(() => {
    vi.resetModules()
    vi.useFakeTimers()
    FakeWebSocket.instances = []
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    vi.useRealTimers()
  })

  it("selects the WebSocket transport when window.__MOBIFLIGHT__.wsUrl is injected", async () => {
    vi.stubGlobal("WebSocket", FakeWebSocket)
    vi.stubGlobal("window", { __MOBIFLIGHT__: { wsUrl: "ws://test/" } })

    const { default: messageClient } = await import("../messageClient")
    messageClient.publish(testCommand)

    expect(FakeWebSocket.instances).toHaveLength(1)
    expect(FakeWebSocket.instances[0].url).toBe("ws://test/")
  })

  it("falls back to the postMessage transport when no wsUrl is available", async () => {
    const postMessage = vi.fn()
    vi.stubGlobal("window", { chrome: { webview: { postMessage, addEventListener: vi.fn(), removeEventListener: vi.fn() } } })

    const { default: messageClient } = await import("../messageClient")
    messageClient.publish(testCommand)

    expect(postMessage).toHaveBeenCalledWith(testCommand)
    expect(FakeWebSocket.instances).toHaveLength(0)
  })

  it("does not re-publish Ready on the very first socket open", async () => {
    vi.stubGlobal("WebSocket", FakeWebSocket)
    vi.stubGlobal("window", { __MOBIFLIGHT__: { wsUrl: "ws://test/" } })

    const { default: messageClient } = await import("../messageClient")
    messageClient.publish(testCommand) // forces the transport (and thus the socket) to be created

    FakeWebSocket.instances[0].triggerOpen()

    expect(FakeWebSocket.instances[0].sent).toEqual([JSON.stringify(testCommand)])
  })

  it("re-publishes CommandFrontendState ready on every open after the first", async () => {
    vi.stubGlobal("WebSocket", FakeWebSocket)
    vi.stubGlobal("window", { __MOBIFLIGHT__: { wsUrl: "ws://test/" } })

    const { default: messageClient } = await import("../messageClient")
    messageClient.publish(testCommand)

    const socket = FakeWebSocket.instances[0]
    socket.triggerOpen() // first open: no resync

    socket.triggerClose()
    vi.advanceTimersByTime(1000) // reconnect backoff
    FakeWebSocket.instances[1].triggerOpen() // reconnect: resync expected

    expect(FakeWebSocket.instances[1].sent).toEqual([
      JSON.stringify({ key: "CommandFrontendState", payload: { route: "/start", state: "ready" } }),
    ])
  })
})
