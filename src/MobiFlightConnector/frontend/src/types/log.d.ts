export type LogLevel = "Debug" | "Info" | "Warn" | "Error" | "Off";

export interface LogEntry {
  Id: string
  Timestamp: string // IsoString, e.g. "2023-08-30T12:34:56.789Z"
  Message: string
  Severity: LogLevel
}