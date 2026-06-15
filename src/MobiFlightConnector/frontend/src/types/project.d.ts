import { ControllerBinding } from "@/types/controller"
import { ConfigFile } from "./config"

export type AircraftInfo = {
  Vendor: string
  Name: string
}

export type SimulatorType = "msfs" | "xplane" | "p3d" | "fsx" | "none"

export interface Project {
  Name: string
  FilePath: string
  ConfigFiles: ConfigFile[]
  Thumbnail?: string
  Sim: SimulatorType
  Features: ProjectFeatures
  ControllerBindings: ControllerBinding[]
  Aircraft?: AircraftInfo[]
}

export interface ProjectInfo {
  Name: string
  FilePath: string

  Thumbnail?: string
  Sim: SimulatorType
  Favorite?: boolean
  Features: ProjectFeatures
  ControllerBindings: ControllerBinding[]
  Aircraft?: AircraftInfo[]
}

export interface ProjectFeatures {
  FSUIPC: boolean
  ProSim: boolean
}