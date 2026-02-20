import { ControllerType } from "./config"

export type Controller = {
  Name: string
  Vendor: string
  ProductId: string
  VendorId: string
  Type: ControllerType
  Connected: boolean
  Serial: string
  ImageUrl: string | null
  certified: boolean
  firmwareUpdate?: boolean
}

export type ControllerBinding = {
  BoundController: Partial<Controller> | null
  Status: ControllerBindingStatus
  OriginalController: Partial<Controller>
}

export type ControllerBindingStatus = "Match" | "AutoBind" | "Missing" | "RequiresManualBind"