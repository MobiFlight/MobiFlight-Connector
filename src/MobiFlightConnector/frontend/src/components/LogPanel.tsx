import { useState, useEffect, useRef, useCallback } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import { AppMessage, LogEntry } from "@/types/messages"
import { ILogMessage, LogLevel } from "@/types/log"
import { useSettingsStore } from "@/stores/settingsStore"
import { useTranslation } from "react-i18next"

const LEVEL_ORDER: Record<LogLevel, number> = {
  trace: 0, debug: 1, info: 2, warn: 3, error: 4, off: 5,
}

const shouldShow = (severity: string, setting: string | undefined): boolean => {
  const effectiveLevel = setting ?? "info"
  if (effectiveLevel === "off") return false
  const entryLevel = LEVEL_ORDER[severity as LogLevel] ?? 2
  const filterLevel = LEVEL_ORDER[effectiveLevel as LogLevel] ?? 2
  return entryLevel >= filterLevel
}

type LogItem = ILogMessage & { id: number }

const SEVERITY_CLASS: Record<string, string> = {
  error: "text-red-500",
  warn:  "text-yellow-500",
  info:  "text-blue-400",
  debug: "text-gray-400",
  trace: "text-gray-300",
}

const LogPanel = () => {
  const { t } = useTranslation()
  const [entries, setEntries] = useState<LogItem[]>([])
  const [height, setHeight] = useState(128)
  const logLevel = useSettingsStore(s => s.settings?.LogLevel)
  const logEnabled = useSettingsStore(s => s.settings?.LogEnabled)
  const scrollRef = useRef<HTMLDivElement>(null)
  const dragStartRef = useRef<{ y: number; height: number } | null>(null)
  const entryCounterRef = useRef(0)
  const dragCleanupRef = useRef<(() => void) | null>(null)

  const onDragHandleMouseDown = (e) => {
    e.preventDefault()
    dragStartRef.current = { y: e.clientY, height }

    const onMouseMove = (e: MouseEvent) => {
      if (!dragStartRef.current) return
      const delta = dragStartRef.current.y - e.clientY
      setHeight(Math.max(80, Math.min(600, dragStartRef.current.height + delta)))
    }

    const cleanup = () => {
      dragStartRef.current = null
      dragCleanupRef.current = null
      document.removeEventListener("mousemove", onMouseMove)
      document.removeEventListener("mouseup", onMouseUp)
    }

    const onMouseUp = () => cleanup()
    dragCleanupRef.current = cleanup

    document.addEventListener("mousemove", onMouseMove)
    document.addEventListener("mouseup", onMouseUp)
  }

  useEffect(() => () => { dragCleanupRef.current?.() }, [])

  const handleMessage = useCallback((msg: AppMessage) => {
    const entry = msg.payload as LogEntry
    setEntries(prev => [...prev.slice(-499), {
      id: entryCounterRef.current++,
      Message: entry.Message,
      Severity: entry.Severity.toLowerCase() as LogLevel,
      Timestamp: new Date(entry.Timestamp),
    }])
  }, [])

  useAppMessage("LogEntry", handleMessage)

  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight
    }
  }, [entries])

  const filtered = entries.filter(e => shouldShow(e.Severity, logLevel))

  return (
    <div className="flex flex-col border-t bg-background">
      <div
        role="separator"
        aria-label="Resize log panel"
        aria-orientation="horizontal"
        className="h-1 cursor-row-resize bg-border hover:bg-primary/50 transition-colors"
        onMouseDown={onDragHandleMouseDown}
      />
      <div className="flex items-center px-3 py-1 text-xs font-medium text-muted-foreground border-b">
        {t("LogPanel.Title")}
      </div>
      <div ref={scrollRef} style={{ height }} className="overflow-y-auto font-mono text-xs p-2 space-y-0.5 select-text">
        {logEnabled === false ? (
          <div className="text-muted-foreground">{t("LogPanel.LoggingDisabled")}</div>
        ) : filtered.length === 0 ? (
          <div className="text-muted-foreground">{t("LogPanel.Empty")}</div>
        ) : (
          filtered.map((entry) => (
            <div key={entry.id} className="flex gap-2">
              <span className="text-muted-foreground shrink-0">
                {entry.Timestamp.toLocaleTimeString()}
              </span>
              <span className={`shrink-0 uppercase ${SEVERITY_CLASS[entry.Severity] ?? ""}`}>
                {entry.Severity}
              </span>
              <span className="break-all">{entry.Message}</span>
            </div>
          ))
        )}
      </div>
    </div>
  )
}

export default LogPanel
