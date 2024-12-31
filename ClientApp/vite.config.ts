import path from "path";
import { TanStackRouterVite } from "@tanstack/router-vite-plugin";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "tailwindcss";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";

// https://vite.dev/config/
export default defineConfig({
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  css: {
    postcss: {
      plugins: [tailwindcss()],
    },
  },
  plugins: [
    react(),
    svgr({
      include: "**/*.svg?react",
    }),
    TanStackRouterVite(),
  ],
  server: {
    // https: {},
    host: "192.168.1.6",
    port: 5173,
    strictPort: true,
  },
});
