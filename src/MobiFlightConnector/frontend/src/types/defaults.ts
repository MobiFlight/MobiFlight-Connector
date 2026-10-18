import { FsuipcOffset, FsuipcSource, ProSimSource, SimConnectSource, VariableSource, XplaneSource } from "@/types/config";

export const FsuipcDefaults = {
  createOffset: () : FsuipcOffset => ({
    OffsetType: 0,
    Offset: 0x66c0,
    Size: 1,
    Mask: 0xff,
    BcdMode: false,
  }),
}

export const SourceDefaults = {
  createSimConnectSource: () : SimConnectSource => ({
    Type: "SimConnectSource",
    SimConnectValue: {
      UUID: "",
      Value: ""
    }
  }),
  createXplaneSource: () : XplaneSource => ({
    Type: "XplaneSource",
    XplaneDataRef: {
      Path: ""
    }
  }),
  createProSimSource: () : ProSimSource => ({
    Type: "ProSimSource",
    ProSimDataRef: {
      Path: ""
    }
  }),
  createVariableSource: () : VariableSource => ({
    Type: "VariableSource",
    MobiFlightVariable: {
      Name: "",
      TYPE: "number",
      Expression: "",
      Number: 0,
      Text: ""
    }
  }),
  createFsuipcSource: () : FsuipcSource => ({
    Type: "FsuipcSource",
    FSUIPC: FsuipcDefaults.createOffset()
  })
}