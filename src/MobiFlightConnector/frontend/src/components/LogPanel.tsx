import { useEffect, useRef, useState } from "react"
import {
  IconChevronDown,
  IconClipboardCopy,
  IconFilter,
  IconLogs,
  IconPlayerPause,
  IconPlayerPlay,
  IconX,
} from "@tabler/icons-react"
import messageExchange from "@/lib/messageExchange"
import { LogLevel } from "@/types/log"
import { useSettingsStore } from "@/stores/settingsStore"
import { useTranslation } from "react-i18next"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { useLogsStore } from "@/stores/logsStore"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import { ScrollArea } from "@/components/ui/scroll-area"
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

const LEVEL_ORDER: Record<LogLevel, number> = {
  debug: 0,
  info: 1,
  warn: 2,
  error: 3,
  off: 4,
}

const shouldShow = (severity: string, setting: string | undefined): boolean => {
  const effectiveLevel = setting ?? "info"

  if (effectiveLevel === "off") return false
  const entryLevel = LEVEL_ORDER[severity as LogLevel] ?? 2
  const filterLevel = LEVEL_ORDER[effectiveLevel as LogLevel] ?? 2
  return entryLevel >= filterLevel
}

type FilterLevel = Exclude<LogLevel, "off">

const FILTER_LEVELS: FilterLevel[] = ["debug", "info", "warn", "error"]

const formatTimestamp = (timestamp: string): string => {
  return `[${timestamp.slice(11, 19)}]`
}

const SEVERITY_CLASS: Record<LogLevel, string> = {
  error: "text-red-500",
  warn: "text-yellow-500",
  info: "text-blue-400",
  debug: "text-gray-400",
  off: "text-gray-300",
}

const LogPanel = () => {
  const { t } = useTranslation()
  const { publish } = messageExchange
  const { logs } = useLogsStore()

  // don't auto scroll, don't append new logs
  const [pauseLog, setPauseLog] = useState(false)
  const [filterText, setFilterText] = useState("")
  // severities checked in the level filter menu; all on by default
  const [visibleLevels, setVisibleLevels] =
    useState<FilterLevel[]>(FILTER_LEVELS)

  const logLevel = useSettingsStore((s) => s.settings?.LogLevel)
  const scrollRef = useRef<HTMLDivElement>(null)

  // any time logs changes, scroll to the bottom of the log panel
  useEffect(() => {
    if (pauseLog) return
    if (scrollRef.current) {
      const scrollArea = scrollRef.current?.querySelector(
        "[data-radix-scroll-area-viewport]",
      )
      if (scrollArea) {
        scrollArea.scrollTop = scrollArea.scrollHeight
      }
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight
    }
  }, [logs, pauseLog])

  const filtered = logs.filter(
    (e) =>
      shouldShow(e.Severity, logLevel) &&
      (!FILTER_LEVELS.includes(e.Severity as FilterLevel) ||
        visibleLevels.includes(e.Severity as FilterLevel)) &&
      (filterText === "" ||
        e.Message.toLowerCase().includes(filterText.toLowerCase())),
  )

  // levels below the log level from the settings never arrive
  const availableLevels = FILTER_LEVELS.filter((l) => shouldShow(l, logLevel))
  const selectedLevels = availableLevels.filter((l) =>
    visibleLevels.includes(l),
  )
  const isLevelFiltering = selectedLevels.length !== availableLevels.length

  const isFiltering = filterText !== "" || isLevelFiltering

  const toggleLevel = (level: FilterLevel) => {
    setVisibleLevels((levels) =>
      levels.includes(level)
        ? levels.filter((l) => l !== level)
        : [...levels, level],
    )
  }

  const levelLabels: Record<FilterLevel, string> = {
    debug: t("Settings.General.Logging.Levels.Debug"),
    info: t("Settings.General.Logging.Levels.Info"),
    warn: t("Settings.General.Logging.Levels.Warn"),
    error: t("Settings.General.Logging.Levels.Error"),
  }
  const levelLabel = (level: FilterLevel) => levelLabels[level]

  // the selected levels are shown in their severity colour
  const levelFilterLabel = (() => {
    if (!isLevelFiltering) return t("LogPanel.Filter.AllLevels")
    if (selectedLevels.length === 0) return t("LogPanel.Filter.NoLevels")
    if (selectedLevels.length === 1) {
      const level = selectedLevels[0]
      return (
        <span className={SEVERITY_CLASS[level]}>
          {t("LogPanel.Filter.LevelOnly", { level: levelLabel(level) })}
        </span>
      )
    }
    return selectedLevels.map((level, index) => (
      <span key={level}>
        {index > 0 && ", "}
        <span className={SEVERITY_CLASS[level]}>{levelLabel(level)}</span>
      </span>
    ))
  })()

  const clearFilters = () => {
    setFilterText("")
    setVisibleLevels(FILTER_LEVELS)
  }

  const toggleLog = () => {
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "view.log.toggle",
      },
    })
  }

  const handleKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) => {
    e.stopPropagation()
  }

  const copyLogToClipboard = () => {
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "extras.copylogs",
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
        <div className="flex flex-row items-center gap-4">
          <div className="flex flex-row items-center gap-2">
            <IconLogs className={"h-6 w-6"} />
            <span>{t("LogPanel.Title")}</span>
            <Separator orientation="vertical" className="h-6" />
            <Button
              onClick={copyLogToClipboard}
              size="sm"
              variant="ghost"
              className="px-2 [&_svg]:size-6"
              title={t("MainMenu.Extras.CopyLogs")}
            >
              <IconClipboardCopy />
              <span className="sr-only">{t("MainMenu.Extras.CopyLogs")}</span>
            </Button>
            <Button
              onClick={() => setPauseLog(!pauseLog)}
              size="sm"
              variant="ghost"
              className="px-2 [&_svg]:size-5"
              title={pauseLog ? t("LogPanel.Resume") : t("LogPanel.Pause")}
            >
              {pauseLog ? <IconPlayerPause /> : <IconPlayerPlay />}
              <span className="sr-only">
                {pauseLog ? t("LogPanel.Resume") : t("LogPanel.Pause")}
              </span>
            </Button>
            <Separator orientation="vertical" className="h-6" />
          </div>
          <div
            className="flex flex-row items-center gap-2"
            onKeyDown={handleKeyDown}
          >
            <IconFilter className="shrink-0" />
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  size="sm"
                  variant="outline"
                  className="h-8 cursor-pointer"
                  aria-label={t("LogPanel.Filter.Level")}
                  title={t("LogPanel.Filter.Level")}
                >
                  <span className="text-sm">{levelFilterLabel}</span>
                  <IconChevronDown className="opacity-50" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="start" className="min-w-48">
                {FILTER_LEVELS.map((level) => {
                  const blockedBySettings = !availableLevels.includes(level)
                  return (
                    <DropdownMenuCheckboxItem
                      key={level}
                      checked={
                        !blockedBySettings && visibleLevels.includes(level)
                      }
                      disabled={blockedBySettings}
                      title={
                        blockedBySettings
                          ? t("LogPanel.Filter.LevelDisabled")
                          : undefined
                      }
                      // keep the menu open so several levels can be toggled
                      onSelect={(e) => e.preventDefault()}
                      onCheckedChange={() => toggleLevel(level)}
                      className="group cursor-pointer"
                    >
                      <span className={cn("grow", SEVERITY_CLASS[level])}>
                        {levelLabel(level)}
                      </span>
                      <button
                        type="button"
                        className="text-muted-foreground hover:text-foreground ml-4 cursor-pointer text-xs opacity-0 group-hover:opacity-100 group-focus:opacity-100"
                        onClick={(e) => {
                          // don't let the row toggle its own checkbox
                          e.preventDefault()
                          e.stopPropagation()
                          setVisibleLevels([level])
                        }}
                      >
                        {t("LogPanel.Filter.Only")}
                      </button>
                    </DropdownMenuCheckboxItem>
                  )
                })}
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  onSelect={() => setVisibleLevels(FILTER_LEVELS)}
                >
                  {t("LogPanel.Filter.ShowAll")}
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
            <Input
              placeholder={t("LogPanel.Filter.Placeholder")}
              value={filterText}
              onChange={(e) => setFilterText(e.target.value)}
            />
            {isFiltering && (
              <Button
                onClick={clearFilters}
                size="sm"
                variant="ghost"
                className="px-2 [&_svg]:size-5"
                title={t("LogPanel.Filter.Clear")}
              >
                <IconX />
                <span className="sr-only">{t("LogPanel.Filter.Clear")}</span>
              </Button>
            )}
          </div>
        </div>
        <Button
          size="sm"
          variant="ghost"
          onMouseDown={(e) => e.stopPropagation()}
          onClick={toggleLog}
          title={t("LogPanel.Close")}
        >
          <IconX size={14} />
        </Button>
      </div>
      {/* Log entries container */}
      <ScrollArea className="grow" ref={scrollRef}>
        <div
          role="log"
          aria-live="polite"
          data-testid="log-panel-content"
          className="flex grow flex-col p-2 font-mono select-text"
        >
          {filtered.length === 0 ? (
            isFiltering ? (
              <div className="text-muted-foreground">
                {t("LogPanel.Filter.NoResults")}
              </div>
            ) : (
              <div className="text-muted-foreground">{t("LogPanel.Empty")}</div>
            )
          ) : (
            filtered.map((entry) => (
              <div
                key={entry.Id}
                className="flex flex-row gap-2"
                data-severity={`${entry.Severity}`}
              >
                <div className="text-muted-foreground">
                  {formatTimestamp(entry.Timestamp)}
                </div>
                <div
                  className={cn(
                    `w-12 uppercase`,
                    SEVERITY_CLASS[entry.Severity] ?? "",
                  )}
                >
                  {entry.Severity}
                </div>
                <div
                  data-testid="log-entry-message"
                  className="truncate whitespace-pre"
                >
                  {entry.Message}
                </div>
              </div>
            ))
          )}
        </div>
      </ScrollArea>
    </div>
  )
}

export default LogPanel
