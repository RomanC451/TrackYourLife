import fs from "fs";
import path from "path";
import { ValidateEnv } from "@julr/vite-plugin-validate-env";
import tailwindcss from "@tailwindcss/vite";
import { tanstackRouter } from "@tanstack/router-vite-plugin";
import react from "@vitejs/plugin-react";
import dotenv from "dotenv";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";

dotenv.config();

export default defineConfig({
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  plugins: [
    ValidateEnv(),
    tailwindcss(),
    react({
      babel: {
        plugins: [["babel-plugin-react-compiler"]],
      },
    }),
    svgr({
      include: "**/*.svg?react",
    }),
    tanstackRouter(),
  ],
  server: {
    host: process.env.VITE_HOST,
    port: 5173,
    strictPort: true,
    https:
      process.env.VITE_HTTPS === "true"
        ? {
            key: fs.readFileSync(path.resolve(__dirname, "./.cert/cert.key")),
            cert: fs.readFileSync(path.resolve(__dirname, "./.cert/cert.crt")),
          }
        : undefined,
  },
  mode: process.env.NODE_ENV || "production",
});
