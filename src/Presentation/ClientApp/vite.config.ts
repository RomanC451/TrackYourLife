import { TanStackRouterVite } from "@tanstack/router-vite-plugin";
import react from "@vitejs/plugin-react-swc";
import path from "path";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";

// https://vitejs.dev/config/
export default defineConfig({
  resolve: {
    alias: {
      "~": path.resolve(__dirname, "./src"),
    },
  },
  plugins: [
    react(),
    svgr({
      // svgrOptions: {
      //   exportType: "named",
      //   ref: true,
      //   svgo: false,
      //   titleProp: true,
      // },
      include: "**/*.svg",
    }),
    TanStackRouterVite(),

    // mkcert({ hosts: ["https://192.168.1.8:5173/"] }),
    // basicSsl({ domains: ["https://192.168.1.8:5173/"], name: "TrackYourLife" }),
  ],
  server: {
    // https: true,
    host: "0.0.0.0",
    port: 5173,
  },
});
