import path from "path"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

export default defineConfig(async ({ mode }) => {
  const devPlugins = [];
  if (mode === 'development') {
    const { i18nextHMRPlugin } = await import('i18next-hmr/vite');
    devPlugins.push(i18nextHMRPlugin({ localesDir: './public/locales' }));
  }

  return {
    plugins: [react()].concat(devPlugins),
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
  }
})
