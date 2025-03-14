export interface Project {
  id: string
  filePath: string
  name: string
  linkedAircraft?: string[]
  status: {
    configs: { files: number, configs: number, status: string }
    devices: { count: number, status: string }
    sim: { name: string, status: string }
  }
}