export interface Project {
  id: string
  filePath: string
  name: string
  status: {
    configs: { files: Number, configs: Number, status: string }
    devices: { count: Number, status: string}
    sim: { name: string, status: string }
  }
}

export const Projects : Project[] = [
  { id : "1", filePath: `C:\\temp`, "name": "MSFS2020 - FBW A320", "status": { configs: { files: 0, configs: 0, status: "New" }, devices: { "count": 5, status: "OK" }, sim: { name : "MSFS2020", status: "OK" } } },
  { id : "2", filePath: `C:\\temp`, "name": "MSFS2020 - FBW A320", "status": { configs: { files: 1, configs: 4, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name : "MSFS2020", status: "OK"  } }},
  { id : "3", filePath: `C:\\temp`, "name": "MSFS2020 - FBW A320", "status": { configs: { files: 2, configs: 6, status: "OK" }, devices: { "count": 5, status: "OK" }, sim: { name : "MSFS2020", status: "OK"  } }},
]