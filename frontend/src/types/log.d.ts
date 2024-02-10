export type LogLevel = "info" | "warn" | "error" | "debug" | "trace" | "off";

export interface ILogMessage {
    message: string
    level: LogLevel
    timestamp: Date
}

