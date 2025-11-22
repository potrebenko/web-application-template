import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";
import vuetify from "vite-plugin-vuetify";
import Fonts from "unplugin-fonts/vite";
import { readFileSync } from 'fs';

// Read package.json
const packageJson = JSON.parse(
  readFileSync('./package.json', 'utf-8')
);

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // Load env file based on `mode` in the current directory
  const env = loadEnv(mode, process.cwd());

  return {
    plugins: [
      vue(),
      Fonts({
        google: {
          families: [
            {
              name: "Roboto",
              styles: "wght@100;300;400;500;700;900",
            },
          ],
        },
      }),
      vuetify({
        autoImport: true,
      }),
    ],
    define: {
      // Stringify all values to ensure proper parsing
      APP_CONFIG: JSON.stringify({
        title: env.VITE_APP_TITLE,
        apiUrl: env.VITE_API_URL,
        version: env.VITE_APP_VERSION,
      }),
      // Add package version
      PACKAGE_VERSION: JSON.stringify(packageJson.version),
    },
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    server: {
      port: 3000,
    },
    css: {
      preprocessorOptions: {
        scss: {
          api: "modern-compiler",
        },
      },
    },
  };
});
