import { ConfigFile } from "./config"

export interface Project {
  Name: string
  FilePath: string
  ConfigFiles: ConfigFile[]
  Thumbnail?: string
  Sim: "msfs" | "xplane" | "p3d" | "fsx" | "none"
  Features: ProjectFeatures
  Controllers?: string[]

  Aircraft?: {
    Name: string
    Filter: string
    Available: boolean
  }[]
}

export interface ProjectInfo {
  Name: string
  FilePath: string

  Thumbnail?: string
  Sim: string
  Favorite?: boolean
  Features: ProjectFeatures
  Controllers?: string[]

  Aircraft?: {
    Name: string
    Filter: string
  }[]
}

export interface ProjectFeatures {
  FSUIPC: boolean
  ProSim: boolean
}
