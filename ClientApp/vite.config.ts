import fs from "fs";
import path from "path";
import { ValidateEnv } from "@julr/vite-plugin-validate-env";
import { tanstackRouter } from "@tanstack/router-vite-plugin";
import react from "@vitejs/plugin-react-swc";
import dotenv from "dotenv";
import tailwindcss from "tailwindcss";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";

dotenv.config();

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
    tanstackRouter(),
    ValidateEnv(),
  ],
  server: {
    host: process.env.VITE_HOST,
    port: 5173,
    strictPort: true,
    https: {
      key: fs.readFileSync(path.resolve(__dirname, "./.cert/cert.key")),
      cert: fs.readFileSync(path.resolve(__dirname, "./.cert/cert.crt")),
    },
  },
  mode: "development",
});
