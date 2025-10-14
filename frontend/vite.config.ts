/// <reference types="vitest" />
import path from "path"
import { defineConfig } from "vite"
import react from "@vitejs/plugin-react"

// https://vite.dev/config/
export default defineConfig({
  esbuild: {
    // drop: ['console', 'debugger'],
  },
  plugins: [
    react({
      // Add this line
      include: "**/*.tsx",
      babel: {
        plugins: ['babel-plugin-react-compiler'],
      },
    }),
  ],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
      lodash: "lodash-es",
    },
  },
  server: {
    watch: {
      usePolling: true,
    },
  },
  test: {
    globals: true,
    environment: "node",
    exclude: ["e2e/*", "node_modules/*", "dist/*", "./tests"],
  },
})
