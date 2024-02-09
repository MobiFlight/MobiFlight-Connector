export default interface IGlobalSettings {
    ArcazeSupportEnabled: boolean
    AutoRetrigger: boolean
    AutoRun: boolean
    AutoLoadLinkedConfig: boolean
    BetaUpdates: boolean
    CommunityFeedback: boolean
    EnableJoystickSupport: boolean
    EnableMidiSupport: boolean
    ExcludedJoysticks: string[]
    ExcludedMidiDevices: string[]
    FwAutoUpdateCheck: boolean
    HubHopAutoCheck: boolean
    IgnoredComPortsList: string
    Language: string
    LogEnabled: boolean
    LogJoystickAxis: boolean
    LogLevel: string
    MinimizeOnAutoRun: boolean
    ModuleSettings: string
    OfflineMode: boolean
    PollInterval: number
    RecentFiles: string[]
    RecentFilesMaxCount: number
    TestTimerInterval: number
}