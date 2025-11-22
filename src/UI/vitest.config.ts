import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath } from 'url'
import { dirname, resolve } from 'path'

const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'happy-dom',
    deps: {
      inline: ['vuetify']
    },
    css: {
      include: [/\.css$/, /\.vue$/]
    }
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
    },
  },
  define: {
    // Mock global variables defined in vite.config.ts
    APP_CONFIG: JSON.stringify({
      title: 'Test App Title',
    }),
    PACKAGE_VERSION: JSON.stringify('0.0.1-test'),
  },
})
