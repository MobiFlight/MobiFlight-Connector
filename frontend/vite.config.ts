import path from "path"
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react({
    // Add this line
    include: "**/*.tsx",
  })],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    watch: {
      usePolling: true
    }
  }
})
