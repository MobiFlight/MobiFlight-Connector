import { Button } from "@/components/ui/button";
import { Switch } from "@/components/ui/switch";
import { useGlobalSettingsStore } from "@/stores/globalSettingsStore";
import { IGlobalSettings } from "@/types";
import { useEffect, useState } from "react";
import _ from "lodash";
import { publishOnMessageExchange } from "@/lib/hooks";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { ComboBox } from "@/components/mobiflight/ComboBox";
import { useBlocker } from "react-router";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { IconChevronDown, IconChevronUp } from "@tabler/icons-react";

export default function SettingsPage() {
  const { settings } = useGlobalSettingsStore();
  const [tempSettings, setTempSettings] = useState<IGlobalSettings>();
  const { publish } = publishOnMessageExchange();
  
  const blocker = useBlocker(
    ({ currentLocation, nextLocation }) =>
      !_.isEqual(tempSettings, settings) &&
      currentLocation.pathname !== nextLocation.pathname
  );

  const onLogLevelChange = (value: string) => {
    setTempSettings({
      ...tempSettings,
      LogLevel: value,
    } as IGlobalSettings);
  };

  const onTestTimerIntervalChange = (value: string) => {
    setTempSettings({
      ...tempSettings,
      TestTimerInterval: Number.parseInt(value || "250"),
    } as IGlobalSettings);
  };

  const onLanguageChange = (value: string) => {
    setTempSettings({
      ...tempSettings,
      Language: value,
    } as IGlobalSettings);
  };

  useEffect(() => {
    setTempSettings(useGlobalSettingsStore.getState().settings);
  }, [settings]);
  /*
        x ArcazeSupportEnabled: boolean
        / AutoRetrigger: boolean
        / AutoRun: boolean
        / AutoLoadLinkedConfig: boolean
        / BetaUpdates: boolean
        / CommunityFeedback: boolean
        / EnableJoystickSupport: boolean
        / EnableMidiSupport: boolean
        ExcludedJoysticks: string[]
        ExcludedMidiDevices: string[]
        / FwAutoUpdateCheck: boolean
        / HubHopAutoCheck: boolean
        IgnoredComPortsList: string
        Language: string
        / LogEnabled: boolean
        / LogJoystickAxis: boolean
        / LogLevel: string
        / MinimizeOnAutoRun: boolean
        x ModuleSettings: string
        x OfflineMode: boolean
        x PollInterval: number
        ! RecentFiles: string[]
        / RecentFilesMaxCount: number
        / TestTimerInterval: number
    */
  return (
    <div className="flex flex-col gap-4 overflow-y-auto">
      {blocker && (
        <AlertDialog open={blocker.state === "blocked"}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>You have unsaved changes</AlertDialogTitle>
              <AlertDialogDescription>
                You will lose all unsaved changes if you continue.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel onClick={() => blocker.reset()}>
                Cancel
              </AlertDialogCancel>
              <AlertDialogAction onClick={() => blocker.proceed()}>
                Continue
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}
      <h2 className="scroll-m-20 text-3xl tracking-tight first:mt-0">
        Global settings
      </h2>
      <div className="flex flex-row gap-4 flex-wrap overflow-y-auto pb-3">
        <Card className="w-[350px]">
          <CardHeader className="flex flex-row gap-2 items-center">
            <div className="flex flex-col mt-0">
              <div className="text-xl font-semibold">General</div>
            </div>
          </CardHeader>
          <CardContent className="flex flex-col gap-4">
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56">Number of recent files</label>
              <Button variant={"outline"} className="flex flex-row p-3">
                {tempSettings?.RecentFilesMaxCount.toString()}
                <div className="flex flex-col">
                <IconChevronUp className="ml-2 h-4 w-4 shrink-0 opacity-50 hover:stroke-primary hover:opacity-100" onClick={()=>setTempSettings({
                    ...tempSettings,
                    RecentFilesMaxCount: Math.min(10,tempSettings!.RecentFilesMaxCount + 1),
                  } as IGlobalSettings)}/>
                <IconChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50  hover:stroke-primary hover:opacity-100" onClick={()=>setTempSettings({
                    ...tempSettings,
                    RecentFilesMaxCount: Math.max(1,tempSettings!.RecentFilesMaxCount - 1),
                  } as IGlobalSettings)}/>
                </div>
              </Button>
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Beta updates
              </label>
              <Switch
                checked={tempSettings?.BetaUpdates}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    BetaUpdates: !tempSettings?.BetaUpdates,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Community feedback
              </label>
              <Switch
                checked={tempSettings?.CommunityFeedback}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    CommunityFeedback: !tempSettings?.CommunityFeedback,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Joystick support
              </label>
              <Switch
                checked={tempSettings?.EnableJoystickSupport}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    EnableJoystickSupport: !tempSettings?.EnableJoystickSupport,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Midi device support
              </label>
              <Switch
                checked={tempSettings?.EnableMidiSupport}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    EnableMidiSupport: !tempSettings?.EnableMidiSupport,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Test speed
              </label>
              <ComboBox
                className="w-32"
                options={[
                  { value: "1000", label: "1 sec" },
                  { value: "500", label: "500ms" },
                  { value: "250", label: "250ms" },
                  { value: "100", label: "100ms" },
                  { value: "50", label: "50ms" },
                ]}
                value={tempSettings?.TestTimerInterval.toString() ?? "250"}
                onSelect={onTestTimerIntervalChange}
              ></ComboBox>
            </div>

            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                UI Language
              </label>
              <ComboBox
                className="w-48"
                options={[
                  { value: "system", label: "System Default" },
                  { value: "en-US", label: "English" },
                  { value: "de-DE", label: "Deutsch" },
                ]}
                value={tempSettings?.Language ?? "system"}
                onSelect={onLanguageChange}
              ></ComboBox>
            </div>
          </CardContent>
        </Card>
        <Card className="w-[350px]">
          <CardHeader>
            <div className="flex flex-col mt-0">
              <div className="text-xl font-semibold">Logging</div>
            </div>
          </CardHeader>
          <CardContent className="flex flex-col gap-4">
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Enabled
              </label>
              <Switch
                checked={tempSettings?.LogEnabled}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    LogEnabled: !tempSettings?.LogEnabled,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Log level
              </label>
              <ComboBox
                className="w-32"
                value={tempSettings?.LogLevel ?? "info"}
                options={[
                  {
                    value: "debug",
                    label: "Debug",
                  },
                  {
                    value: "info",
                    label: "Info",
                  },
                  {
                    value: "warn",
                    label: "Warning",
                  },
                  {
                    value: "error",
                    label: "Error",
                  },
                ]}
                onSelect={onLogLevelChange}
              ></ComboBox>
            </div>

            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Log joystick axis
              </label>
              <Switch
                checked={tempSettings?.LogJoystickAxis}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    LogJoystickAxis: !tempSettings?.LogJoystickAxis,
                  } as IGlobalSettings)
                }
              />
            </div>
          </CardContent>
        </Card>
        <Card className="w-[350px]">
          <CardHeader>
            <div className="flex flex-col mt-0">
              <div className="text-xl font-semibold">Run options</div>
            </div>
          </CardHeader>
          <CardContent className="flex flex-col gap-4">
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Auto retrigger
              </label>
              <Switch
                checked={tempSettings?.AutoRetrigger}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    AutoRetrigger: !tempSettings?.AutoRetrigger,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Auto run
              </label>
              <Switch
                checked={tempSettings?.AutoRun}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    AutoRun: !tempSettings?.AutoRun,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Auto load linked config
              </label>
              <Switch
                checked={tempSettings?.AutoLoadLinkedConfig}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    AutoLoadLinkedConfig: !tempSettings?.AutoLoadLinkedConfig,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Firmware auto update check
              </label>
              <Switch
                checked={tempSettings?.FwAutoUpdateCheck}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    FwAutoUpdateCheck: !tempSettings?.FwAutoUpdateCheck,
                  } as IGlobalSettings)
                }
              />
            </div>
            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Hub hop auto check
              </label>
              <Switch
                checked={tempSettings?.HubHopAutoCheck}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    HubHopAutoCheck: !tempSettings?.HubHopAutoCheck,
                  } as IGlobalSettings)
                }
              />
            </div>

            <div className="flex flex-row gap-4 items-center justify-between">
              <label className="w-56" htmlFor="">
                Minimize on auto run
              </label>
              <Switch
                checked={tempSettings?.MinimizeOnAutoRun}
                onClick={() =>
                  setTempSettings({
                    ...tempSettings,
                    MinimizeOnAutoRun: !tempSettings?.MinimizeOnAutoRun,
                  } as IGlobalSettings)
                }
              />
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="flex flex-row gap-4">
        <Button
          variant={"default"}
          className="dark:text-white"
          disabled={_.isEqual(tempSettings, settings)}
          onClick={() => {
            if (!tempSettings) return;
            console.log(tempSettings);
            publish({ key: "GlobalSettingsUpdate", payload: tempSettings });
          }}
        >
          Save changes
        </Button>
      </div>
    </div>
  );
}
