// vite.config.ts
import fs from "fs";
import path from "path";
import { ValidateEnv } from "file:///D:/TrackYourLife/ClientApp/node_modules/@julr/vite-plugin-validate-env/dist/index.js";
import { tanstackRouter } from "file:///D:/TrackYourLife/ClientApp/node_modules/@tanstack/router-vite-plugin/dist/esm/index.js";
import react from "file:///D:/TrackYourLife/ClientApp/node_modules/@vitejs/plugin-react-swc/index.mjs";
import dotenv from "file:///D:/TrackYourLife/ClientApp/node_modules/dotenv/lib/main.js";
import tailwindcss from "file:///D:/TrackYourLife/ClientApp/node_modules/tailwindcss/lib/index.js";
import { defineConfig } from "file:///D:/TrackYourLife/ClientApp/node_modules/vite/dist/node/index.js";
import svgr from "file:///D:/TrackYourLife/ClientApp/node_modules/vite-plugin-svgr/dist/index.js";
var __vite_injected_original_dirname = "D:\\TrackYourLife\\ClientApp";
dotenv.config();
var vite_config_default = defineConfig({
  resolve: {
    alias: {
      "@": path.resolve(__vite_injected_original_dirname, "./src")
    }
  },
  css: {
    postcss: {
      plugins: [tailwindcss()]
    }
  },
  plugins: [
    react(),
    svgr({
      include: "**/*.svg?react"
    }),
    tanstackRouter(),
    ValidateEnv()
  ],
  server: {
    host: process.env.VITE_HOST,
    port: 5173,
    strictPort: true,
    https: {
      key: fs.readFileSync(path.resolve(__vite_injected_original_dirname, "./.cert/cert.key")),
      cert: fs.readFileSync(path.resolve(__vite_injected_original_dirname, "./.cert/cert.crt"))
    }
  },
  mode: "production"
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJEOlxcXFxUcmFja1lvdXJMaWZlXFxcXENsaWVudEFwcFwiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRDpcXFxcVHJhY2tZb3VyTGlmZVxcXFxDbGllbnRBcHBcXFxcdml0ZS5jb25maWcudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0Q6L1RyYWNrWW91ckxpZmUvQ2xpZW50QXBwL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IGZzIGZyb20gXCJmc1wiO1xyXG5pbXBvcnQgcGF0aCBmcm9tIFwicGF0aFwiO1xyXG5pbXBvcnQgeyBWYWxpZGF0ZUVudiB9IGZyb20gXCJAanVsci92aXRlLXBsdWdpbi12YWxpZGF0ZS1lbnZcIjtcclxuaW1wb3J0IHsgdGFuc3RhY2tSb3V0ZXIgfSBmcm9tIFwiQHRhbnN0YWNrL3JvdXRlci12aXRlLXBsdWdpblwiO1xyXG5pbXBvcnQgcmVhY3QgZnJvbSBcIkB2aXRlanMvcGx1Z2luLXJlYWN0LXN3Y1wiO1xyXG5pbXBvcnQgZG90ZW52IGZyb20gXCJkb3RlbnZcIjtcclxuaW1wb3J0IHRhaWx3aW5kY3NzIGZyb20gXCJ0YWlsd2luZGNzc1wiO1xyXG5pbXBvcnQgeyBkZWZpbmVDb25maWcgfSBmcm9tIFwidml0ZVwiO1xyXG5pbXBvcnQgc3ZnciBmcm9tIFwidml0ZS1wbHVnaW4tc3ZnclwiO1xyXG5cclxuZG90ZW52LmNvbmZpZygpO1xyXG5cclxuZXhwb3J0IGRlZmF1bHQgZGVmaW5lQ29uZmlnKHtcclxuICByZXNvbHZlOiB7XHJcbiAgICBhbGlhczoge1xyXG4gICAgICBcIkBcIjogcGF0aC5yZXNvbHZlKF9fZGlybmFtZSwgXCIuL3NyY1wiKSxcclxuICAgIH0sXHJcbiAgfSxcclxuICBjc3M6IHtcclxuICAgIHBvc3Rjc3M6IHtcclxuICAgICAgcGx1Z2luczogW3RhaWx3aW5kY3NzKCldLFxyXG4gICAgfSxcclxuICB9LFxyXG4gIHBsdWdpbnM6IFtcclxuICAgIHJlYWN0KCksXHJcbiAgICBzdmdyKHtcclxuICAgICAgaW5jbHVkZTogXCIqKi8qLnN2Zz9yZWFjdFwiLFxyXG4gICAgfSksXHJcbiAgICB0YW5zdGFja1JvdXRlcigpLFxyXG4gICAgVmFsaWRhdGVFbnYoKSxcclxuICBdLFxyXG4gIHNlcnZlcjoge1xyXG4gICAgaG9zdDogcHJvY2Vzcy5lbnYuVklURV9IT1NULFxyXG4gICAgcG9ydDogNTE3MyxcclxuICAgIHN0cmljdFBvcnQ6IHRydWUsXHJcbiAgICBodHRwczoge1xyXG4gICAgICBrZXk6IGZzLnJlYWRGaWxlU3luYyhwYXRoLnJlc29sdmUoX19kaXJuYW1lLCBcIi4vLmNlcnQvY2VydC5rZXlcIikpLFxyXG4gICAgICBjZXJ0OiBmcy5yZWFkRmlsZVN5bmMocGF0aC5yZXNvbHZlKF9fZGlybmFtZSwgXCIuLy5jZXJ0L2NlcnQuY3J0XCIpKSxcclxuICAgIH0sXHJcbiAgfSxcclxuICBtb2RlOiBcInByb2R1Y3Rpb25cIixcclxufSk7XHJcbiJdLAogICJtYXBwaW5ncyI6ICI7QUFBc1EsT0FBTyxRQUFRO0FBQ3JSLE9BQU8sVUFBVTtBQUNqQixTQUFTLG1CQUFtQjtBQUM1QixTQUFTLHNCQUFzQjtBQUMvQixPQUFPLFdBQVc7QUFDbEIsT0FBTyxZQUFZO0FBQ25CLE9BQU8saUJBQWlCO0FBQ3hCLFNBQVMsb0JBQW9CO0FBQzdCLE9BQU8sVUFBVTtBQVJqQixJQUFNLG1DQUFtQztBQVV6QyxPQUFPLE9BQU87QUFFZCxJQUFPLHNCQUFRLGFBQWE7QUFBQSxFQUMxQixTQUFTO0FBQUEsSUFDUCxPQUFPO0FBQUEsTUFDTCxLQUFLLEtBQUssUUFBUSxrQ0FBVyxPQUFPO0FBQUEsSUFDdEM7QUFBQSxFQUNGO0FBQUEsRUFDQSxLQUFLO0FBQUEsSUFDSCxTQUFTO0FBQUEsTUFDUCxTQUFTLENBQUMsWUFBWSxDQUFDO0FBQUEsSUFDekI7QUFBQSxFQUNGO0FBQUEsRUFDQSxTQUFTO0FBQUEsSUFDUCxNQUFNO0FBQUEsSUFDTixLQUFLO0FBQUEsTUFDSCxTQUFTO0FBQUEsSUFDWCxDQUFDO0FBQUEsSUFDRCxlQUFlO0FBQUEsSUFDZixZQUFZO0FBQUEsRUFDZDtBQUFBLEVBQ0EsUUFBUTtBQUFBLElBQ04sTUFBTSxRQUFRLElBQUk7QUFBQSxJQUNsQixNQUFNO0FBQUEsSUFDTixZQUFZO0FBQUEsSUFDWixPQUFPO0FBQUEsTUFDTCxLQUFLLEdBQUcsYUFBYSxLQUFLLFFBQVEsa0NBQVcsa0JBQWtCLENBQUM7QUFBQSxNQUNoRSxNQUFNLEdBQUcsYUFBYSxLQUFLLFFBQVEsa0NBQVcsa0JBQWtCLENBQUM7QUFBQSxJQUNuRTtBQUFBLEVBQ0Y7QUFBQSxFQUNBLE1BQU07QUFDUixDQUFDOyIsCiAgIm5hbWVzIjogW10KfQo=
