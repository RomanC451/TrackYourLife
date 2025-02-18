import path from "path";
import { ValidateEnv } from "@julr/vite-plugin-validate-env";
import { TanStackRouterVite } from "@tanstack/router-vite-plugin";
import react from "@vitejs/plugin-react-swc";
import dotenv from "dotenv";
import tailwindcss from "tailwindcss";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";

dotenv.config();

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
    ValidateEnv(),
  ],
  server: {
    // https: {},
    host: process.env.VITE_HOST,
    port: 5173,
    strictPort: true,
  },
});
