export type LogLevel = "debug" | "info" | "warn" | "error" | "off";

export interface LogEntry {
  Id: string
  Timestamp: string // IsoString, e.g. "2023-08-30T12:34:56.789Z"
  Message: string
  Severity: LogLevel
}