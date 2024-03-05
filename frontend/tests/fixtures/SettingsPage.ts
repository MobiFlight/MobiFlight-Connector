import { MFPage } from "./mfpage";
import * as Types from "../../src/types/index";

export class SettingsPage {
  settings: Types.IGlobalSettings = {} as Types.IGlobalSettings;

  constructor(public readonly mfPage: MFPage) {
    this.mfPage.page.addInitScript(() => {
      // we register this special event listener
      // to listen for the GlobalSettingsUpdate message
      // and create a fake message as if it came from the backend
      window.addEventListener("message", (event): void => {
        const message = event.data as Types.GlobalSettingsUpdateMessage;
        if (message.key === "GlobalSettingsUpdate") {
          // Global Settings Update was submitted from the frontend
          // and we repack it as if it came from the backend
          const backendResponse = {
            key: "GlobalSettings",
            payload: message.payload,
          };

          // All messages coming from the backend are stringified
          // so we need to do the same
          const stringifiedObject = JSON.stringify(backendResponse);

          // and we post the message back to the "frontend"
          window.postMessage(stringifiedObject, "*");
        }
      });
    });
  }

  getDefaultSettings(): Types.IGlobalSettings {
    return {
      ArcazeSupportEnabled: true,
      AutoRetrigger: true,
      AutoRun: true,
      AutoLoadLinkedConfig: true,
      BetaUpdates: true,
      CommunityFeedback: true,
      EnableJoystickSupport: true,
      EnableMidiSupport: true,
      ExcludedJoysticks: [],
      ExcludedMidiBoards: [],
      FwAutoUpdateCheck: true,
      HubHopAutoCheck: true,
      IgnoredComPortsList: "",
      Language: "system",
      LogEnabled: true,
      LogJoystickAxis: true,
      LogLevel: "Debug",
      MinimizeOnAutoRun: true,
      ModuleSettings: "",
      RecentFiles: [],
      RecentFilesMaxCount: 10,
      TestTimerInterval: 1000,
    };
  }

  async loadDefaultSettings() {
    await this.UpdateSettings(this.getDefaultSettings());
  }

  async UpdateSettings(settings: Types.IGlobalSettings) {
    const message: Types.AppMessage = {
      key: "GlobalSettings",
      payload: { ...settings },
    };
    await this.mfPage.publishMessage(message);
  }
}
