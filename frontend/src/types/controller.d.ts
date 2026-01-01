import { ControllerType } from "./config"

export type Controller = {
  Name: string
  Vendor: string
  ProductId: string
  VendorId: string
  Type: ControllerType
  Connected: boolean
  ImageUrl: string | null
  certified: boolean
  firmwareUpdate?: boolean
}

export type ControllerBinding = {
  BoundController: string
  Status: ControllerBindingStatus
  OriginalController: string | null
}

export type ControllerBindingStatus = "Match" | "AutoBind" | "Missing" | "RequiresManualBind"