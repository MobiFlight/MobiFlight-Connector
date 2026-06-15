import { ControllerBinding } from "@/types/controller"
import { ConfigFile } from "./config"

export type AircraftInfo = {
  Vendor: string
  Name: string
}

export interface Project {
  Name: string
  FilePath: string
  ConfigFiles: ConfigFile[]
  Thumbnail?: string
  Sim: "msfs" | "xplane" | "p3d" | "fsx" | "none"
  Features: ProjectFeatures
  ControllerBindings: ControllerBinding[]
  Aircraft?: AircraftInfo[]
}

export interface ProjectInfo {
  Name: string
  FilePath: string

  Thumbnail?: string
  Sim: string
  Favorite?: boolean
  Features: ProjectFeatures
  ControllerBindings: ControllerBinding[]
  Aircraft?: AircraftInfo[]
}

export interface ProjectFeatures {
  FSUIPC: boolean
  ProSim: boolean
}