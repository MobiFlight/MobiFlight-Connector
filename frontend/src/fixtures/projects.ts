export interface Project {
  id: string
  filePath: string
  name: string
  linkedAircraft?: string[]
  status: {
    configs: { files: Number, configs: Number, status: string }
    devices: { count: Number, status: string }
    sim: { name: string, status: string }
  }
}

export const Projects: Project[] = [
  { id: "1", filePath: `C:\\temp`, name: "FBW A320", linkedAircraft: [ "FBW A320*"], status: { configs: { files: 0, configs: 0, status: "New" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "2", filePath: `C:\\temp`, name: "PMDG B737", linkedAircraft: [ "PMDG B737-800", "PMDG B737-900"], status: { configs: { files: 1, configs: 4, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "3", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "4", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "5", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "6", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "7", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "8", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "9", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "10", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
  { id: "11", filePath: `C:\\temp`, name: "C172", status: { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name: "MSFS2020", status: "OK" } } },
]