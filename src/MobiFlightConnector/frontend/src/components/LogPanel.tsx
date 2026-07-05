import { useState, useEffect, useRef, useCallback } from "react"
import { IconX } from "@tabler/icons-react"
import { publishOnMessageExchange, useAppMessage } from "@/lib/hooks/appMessage"
import { AppMessage, LogEntry } from "@/types/messages"
import { ILogMessage, LogLevel } from "@/types/log"
import { useSettingsStore } from "@/stores/settingsStore"
import { useTranslation } from "react-i18next"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { useLogsStore } from "@/stores/logsStore"

const LEVEL_ORDER: Record<LogLevel, number> = {
  trace: 0,
  debug: 1,
  info: 2,
  warn: 3,
  error: 4,
  off: 5,
}

// Cap on retained log entries. The newest entry is appended after slicing,
// so we keep MAX_ENTRIES - 1 of the previous ones to land exactly at the cap.
const MAX_ENTRIES = 500

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
  warn: "text-yellow-500",
  info: "text-blue-400",
  debug: "text-gray-400",
  trace: "text-gray-300",
}

const LogPanel = () => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const { logs } = useLogsStore()

  const [entries, setEntries] = useState<LogItem[]>(
    logs.map((log, index) => ({
      ...log,
      id: index,
      Severity: log.Severity.toLowerCase() as LogLevel,
      Timestamp: new Date(log.Timestamp),
    })),
  )
  const logLevel = useSettingsStore((s) => s.settings?.LogLevel)
  const scrollRef = useRef<HTMLDivElement>(null)

  // Start counter at the number of existing logs
  const entryCounterRef = useRef(logs.length)

  const handleMessage = useCallback((msg: AppMessage) => {
    const entry = msg.payload as LogEntry
    setEntries((prev) => [
      ...prev.slice(-(MAX_ENTRIES - 1)),
      {
        id: entryCounterRef.current++,
        Message: entry.Message,
        Severity: entry.Severity.toLowerCase() as LogLevel,
        Timestamp: new Date(entry.Timestamp),
      },
    ])
  }, [])

  useAppMessage("LogEntry", handleMessage)

  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight
    }
  }, [entries])

  const filtered = entries.filter((e) => shouldShow(e.Severity, logLevel))

  const toggleLog = () => {
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "view.log.toggle",
      },
    })
  }

  return (
    <div
      className="bg-background flex grow flex-col overflow-hidden"
      data-testid="log-panel"
    >
      {/* Title bar with close button */}
      <div
        data-testid="log-panel-titlebar"
        className="text-muted-foreground flex flex-row items-center justify-between border-b px-3 py-1 font-medium"
      >
        <span>{t("LogPanel.Title")}</span>
        <Button
          size="sm"
          variant="ghost"
          onMouseDown={(e) => e.stopPropagation()}
          onClick={toggleLog}
          aria-label="Close log panel"
        >
          <IconX size={14} />
        </Button>
      </div>
      {/* Log entries container */}
      <div
        role="log"
        aria-live="polite"
        ref={scrollRef}
        data-testid="log-panel-content"
        className="flex flex-col overflow-y-auto p-2 font-mono select-text"
      >
        {filtered.length === 0 ? (
          <div className="text-muted-foreground">{t("LogPanel.Empty")}</div>
        ) : (
          filtered.map((entry) => (
            <div
              key={entry.id}
              className="flex flex-row gap-2"
              data-severity={`${entry.Severity}`}
            >
              <div className="text-muted-foreground">
                {entry.Timestamp.toLocaleTimeString()}
              </div>
              <div
                className={cn(
                  `uppercase`,
                  SEVERITY_CLASS[entry.Severity] ?? "",
                )}
              >
                {entry.Severity}
              </div>
              <div className="truncate">{entry.Message}</div>
            </div>
          ))
        )}
      </div>
    </div>
  )
}

export default LogPanel
