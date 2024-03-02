import { MFPage } from "./mfpage";
import * as Types from "../../src/types/index";
import { TestJoystick, TestMidiDevice, TestMobiflightBoard } from "./data/devices";

export class DeviceDetailPage {
  constructor(public readonly mfPage: MFPage) {}

  async useDefaultDeviceList() {
    const devices: Types.IDeviceItem[] = [TestMobiflightBoard, TestJoystick, TestMidiDevice];
    await this.postDeviceUpdate(devices);
  }

  async postDeviceUpdate(devices: Types.IDeviceItem[]) {
    const message: Types.AppMessage = {
      key: "DeviceUpdate",
      payload: { Devices: devices },
    };
    await this.mfPage.publishMessage(message);
  }
}
