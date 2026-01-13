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
  BoundController: string | null
  Status: ControllerBindingStatus
  OriginalController: string | null
}

export type ControllerBindingStatus = "Match" | "AutoBind" | "Missing" | "RequiresManualBind"