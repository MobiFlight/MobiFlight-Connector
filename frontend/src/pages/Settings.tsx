import { Switch } from "@/components/ui/switch"
import { useGlobalSettingsStore } from "@/stores/globalSettingsStore"
import { IGlobalSettings } from "@/types"
import { useEffect, useState } from "react"

export default function SettingsPage() {
    const { settings } = useGlobalSettingsStore()
    const [ tempSettings, setTempSettings ] = useState<IGlobalSettings>()

    useEffect(() => {
        setTempSettings(useGlobalSettingsStore.getState().settings)
    }, [settings])
/*
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
*/

    return <div className="flex flex-col gap-4">
        <h2 className='scroll-m-20 text-3xl tracking-tight first:mt-0'>Global settings</h2>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.ArcazeSupportEnabled} onChange={() => setTempSettings({
               ...tempSettings,
                ArcazeSupportEnabled: tempSettings?.ArcazeSupportEnabled
            } as IGlobalSettings
            )} />
            <label htmlFor="">Arcaze support enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.AutoRetrigger} onChange={() => setTempSettings({
               ...tempSettings,
                AutoRetrigger: tempSettings?.AutoRetrigger
            } as IGlobalSettings
            )} />
            <label htmlFor="">Auto retrigger enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.AutoRun} onChange={() => setTempSettings({
               ...tempSettings,
                AutoRun: tempSettings?.AutoRun
            } as IGlobalSettings
            )} />
            <label htmlFor="">Auto run enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.AutoLoadLinkedConfig} onChange={() => setTempSettings({
               ...tempSettings,
                AutoLoadLinkedConfig: tempSettings?.AutoLoadLinkedConfig
            } as IGlobalSettings
            )} />
            <label htmlFor="">Auto load linked config enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.BetaUpdates} onChange={() => setTempSettings({
               ...tempSettings,
                BetaUpdates: tempSettings?.BetaUpdates
            } as IGlobalSettings
            )} />
            <label htmlFor="">Beta updates enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.CommunityFeedback} onChange={() => setTempSettings({
               ...tempSettings,
                CommunityFeedback: tempSettings?.CommunityFeedback
            } as IGlobalSettings
            )} />
            <label htmlFor="">Community feedback enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.EnableJoystickSupport} onChange={() => setTempSettings({
               ...tempSettings,
                EnableJoystickSupport: tempSettings?.EnableJoystickSupport
            } as IGlobalSettings
            )} />
            <label htmlFor="">Enable joystick support</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.EnableMidiSupport} onChange={() => setTempSettings({
               ...tempSettings,
                EnableMidiSupport: tempSettings?.EnableMidiSupport
            } as IGlobalSettings
            )} />
            <label htmlFor="">Enable midi support</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.FwAutoUpdateCheck} onChange={() => setTempSettings({
               ...tempSettings,
                FwAutoUpdateCheck: tempSettings?.FwAutoUpdateCheck
            } as IGlobalSettings
            )} />
            <label htmlFor="">Firmware auto update check</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.HubHopAutoCheck} onChange={() => setTempSettings({
               ...tempSettings,
                HubHopAutoCheck: tempSettings?.HubHopAutoCheck
            } as IGlobalSettings
            )} />
            <label htmlFor="">Hub hop auto check</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.LogEnabled} onChange={() => setTempSettings({
               ...tempSettings,
                LogEnabled: tempSettings?.LogEnabled
            } as IGlobalSettings
            )} />
            <label htmlFor="">Log enabled</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.LogJoystickAxis} onChange={() => setTempSettings({
               ...tempSettings,
                LogJoystickAxis: tempSettings?.LogJoystickAxis
            } as IGlobalSettings
            )} />
            <label htmlFor="">Log joystick axis</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.MinimizeOnAutoRun} onChange={() => setTempSettings({
               ...tempSettings,
                MinimizeOnAutoRun: tempSettings?.MinimizeOnAutoRun
            } as IGlobalSettings
            )} />
            <label htmlFor="">Minimize on auto run</label>
        </div>
        <div className="flex flex-row gap-4 items-center">
            <Switch checked={tempSettings?.AutoLoadLinkedConfig} onChange={() => setTempSettings({
               ...tempSettings,
                AutoLoadLinkedConfig: tempSettings?.AutoLoadLinkedConfig
            } as IGlobalSettings
            )} />
            <label htmlFor="">Auto load linked config</label>
        </div>
    </div>
}