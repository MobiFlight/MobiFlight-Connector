export interface KeyAccelerator {
  key: string
  ctrlKey?: boolean
  altKey?: boolean
  shiftKey?: boolean
  metaKey?: boolean
  action: CommandMainMenuPayload["action"]
  description?: string
}
