export type HistoryAction =
  | string
  | {
      key: string
      params?: Record<string, string | number>
    }
