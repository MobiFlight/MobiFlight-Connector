export type LogLevel = "info" | "warn" | "error" | "debug" | "trace" | "off";
export interface ILogMessage {
    Message: string
    Severity: LogLevel
    Timestamp: Date
}

