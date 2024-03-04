import path from "path"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

export default defineConfig(async ({ mode }) => {
  const devPlugins = [];
  
  return {
    plugins: [react()].concat(devPlugins),
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
  }
})
