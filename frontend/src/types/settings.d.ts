export default interface Settings {
  ArcazeSupportEnabled: boolean
  AutoRetrigger: boolean
  AutoRun: boolean
  AutoLoadLinkedConfig: boolean
  BetaUpdates: boolean
  CommunityFeedback: boolean
  EnableJoystickSupport: boolean
  EnableMidiSupport: boolean
  ExcludedJoysticks: string[]
  ExcludedMidiBoards: string[]
  FwAutoUpdateCheck: boolean
  HubHopAutoCheck: boolean
  IgnoredComPortsList: string
  Language: string
  LogEnabled: boolean
  LogJoystickAxis: boolean
  LogLevel: string
  MinimizeOnAutoRun: boolean
  ModuleSettings: string
  RecentFiles: string[]
  RecentFilesMaxCount: number
  TestTimerInterval: number
}