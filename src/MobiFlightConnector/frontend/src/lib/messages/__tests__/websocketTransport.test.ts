import { describe, it, expect, beforeEach, afterEach, vi } from "vitest"
import { createWebSocketTransport } from "../websocketTransport"
import { CommandOpenLinkInBrowser } from "@/types/commands"
import { AppMessage } from "@/types/messages"

const testCommand: CommandOpenLinkInBrowser = {
  key: "CommandOpenLinkInBrowser",
  payload: { url: "https://example.test" },
}

class FakeWebSocket {
  static readonly CONNECTING = 0
  static readonly OPEN = 1
  static readonly CLOSING = 2
  static readonly CLOSED = 3
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
    this.readyState = FakeWebSocket.CLOSED
    this.onclose?.()
  }

  triggerMessage(data: string) {
    this.onmessage?.({ data })
  }
}

describe("createWebSocketTransport", () => {
  beforeEach(() => {
    FakeWebSocket.instances = []
    vi.stubGlobal("WebSocket", FakeWebSocket)
    vi.useFakeTimers()
  })

  afterEach(() => {
    vi.useRealTimers()
    vi.unstubAllGlobals()
  })

  it("connects immediately on creation", () => {
    createWebSocketTransport("ws://test/")
    expect(FakeWebSocket.instances).toHaveLength(1)
    expect(FakeWebSocket.instances[0].url).toBe("ws://test/")
  })

  it("buffers outbound messages while not open, and flushes them on open", () => {
    const transport = createWebSocketTransport("ws://test/")
    const socket = FakeWebSocket.instances[0]

    transport.send(testCommand)
    expect(socket.sent).toHaveLength(0)

    socket.triggerOpen()
    expect(socket.sent).toEqual([JSON.stringify(testCommand)])
  })

  it("sends immediately once already open", () => {
    const transport = createWebSocketTransport("ws://test/")
    const socket = FakeWebSocket.instances[0]
    socket.triggerOpen()

    transport.send(testCommand)
    expect(socket.sent).toEqual([JSON.stringify(testCommand)])
  })

  it("dispatches incoming messages to onMessage listeners, and unsubscribe stops delivery", () => {
    const transport = createWebSocketTransport("ws://test/")
    const socket = FakeWebSocket.instances[0]

    const received: AppMessage[] = []
    const unsubscribe = transport.onMessage((message) => received.push(message))

    const message: AppMessage = { key: "StatusBarUpdate", payload: { Text: "hi", Value: 1 } }
    socket.triggerMessage(JSON.stringify(message))
    expect(received).toEqual([message])

    unsubscribe()
    socket.triggerMessage(JSON.stringify({ ...message, payload: { Text: "bye", Value: 2 } }))
    expect(received).toHaveLength(1)
  })

  it("reconnects with capped exponential backoff after a close", () => {
    createWebSocketTransport("ws://test/")
    expect(FakeWebSocket.instances).toHaveLength(1)

    FakeWebSocket.instances[0].triggerClose()

    vi.advanceTimersByTime(999)
    expect(FakeWebSocket.instances).toHaveLength(1)
    vi.advanceTimersByTime(1)
    expect(FakeWebSocket.instances).toHaveLength(2) // first retry at 1s

    FakeWebSocket.instances[1].triggerClose()

    vi.advanceTimersByTime(1999)
    expect(FakeWebSocket.instances).toHaveLength(2)
    vi.advanceTimersByTime(1)
    expect(FakeWebSocket.instances).toHaveLength(3) // second retry at 2s (doubled)
  })

  it("resets backoff to the initial delay after a successful open", () => {
    createWebSocketTransport("ws://test/")
    FakeWebSocket.instances[0].triggerClose()
    vi.advanceTimersByTime(1000)
    expect(FakeWebSocket.instances).toHaveLength(2)

    FakeWebSocket.instances[1].triggerOpen() // success resets backoff to 1s
    FakeWebSocket.instances[1].triggerClose()

    vi.advanceTimersByTime(999)
    expect(FakeWebSocket.instances).toHaveLength(2)
    vi.advanceTimersByTime(1)
    expect(FakeWebSocket.instances).toHaveLength(3) // back to 1s, not 4s
  })

  it("fires onOpen listeners on every successful connect, including reconnects", () => {
    const transport = createWebSocketTransport("ws://test/")
    let openCount = 0
    transport.onOpen(() => openCount++)

    FakeWebSocket.instances[0].triggerOpen()
    expect(openCount).toBe(1)

    FakeWebSocket.instances[0].triggerClose()
    vi.advanceTimersByTime(1000)
    FakeWebSocket.instances[1].triggerOpen()
    expect(openCount).toBe(2)
  })
})
