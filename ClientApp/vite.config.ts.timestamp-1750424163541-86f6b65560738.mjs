// vite.config.ts
import fs from "fs";
import path from "path";
import { ValidateEnv } from "file:///D:/TrackYourLife/ClientApp/node_modules/@julr/vite-plugin-validate-env/dist/index.js";
import { TanStackRouterVite } from "file:///D:/TrackYourLife/ClientApp/node_modules/@tanstack/router-vite-plugin/dist/esm/index.js";
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
    TanStackRouterVite(),
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
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJEOlxcXFxUcmFja1lvdXJMaWZlXFxcXENsaWVudEFwcFwiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRDpcXFxcVHJhY2tZb3VyTGlmZVxcXFxDbGllbnRBcHBcXFxcdml0ZS5jb25maWcudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0Q6L1RyYWNrWW91ckxpZmUvQ2xpZW50QXBwL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IGZzIGZyb20gXCJmc1wiO1xyXG5pbXBvcnQgcGF0aCBmcm9tIFwicGF0aFwiO1xyXG5pbXBvcnQgeyBWYWxpZGF0ZUVudiB9IGZyb20gXCJAanVsci92aXRlLXBsdWdpbi12YWxpZGF0ZS1lbnZcIjtcclxuaW1wb3J0IHsgVGFuU3RhY2tSb3V0ZXJWaXRlIH0gZnJvbSBcIkB0YW5zdGFjay9yb3V0ZXItdml0ZS1wbHVnaW5cIjtcclxuaW1wb3J0IHJlYWN0IGZyb20gXCJAdml0ZWpzL3BsdWdpbi1yZWFjdC1zd2NcIjtcclxuaW1wb3J0IGRvdGVudiBmcm9tIFwiZG90ZW52XCI7XHJcbmltcG9ydCB0YWlsd2luZGNzcyBmcm9tIFwidGFpbHdpbmRjc3NcIjtcclxuaW1wb3J0IHsgZGVmaW5lQ29uZmlnIH0gZnJvbSBcInZpdGVcIjtcclxuaW1wb3J0IHN2Z3IgZnJvbSBcInZpdGUtcGx1Z2luLXN2Z3JcIjtcclxuXHJcbmRvdGVudi5jb25maWcoKTtcclxuXHJcbmV4cG9ydCBkZWZhdWx0IGRlZmluZUNvbmZpZyh7XHJcbiAgcmVzb2x2ZToge1xyXG4gICAgYWxpYXM6IHtcclxuICAgICAgXCJAXCI6IHBhdGgucmVzb2x2ZShfX2Rpcm5hbWUsIFwiLi9zcmNcIiksXHJcbiAgICB9LFxyXG4gIH0sXHJcbiAgY3NzOiB7XHJcbiAgICBwb3N0Y3NzOiB7XHJcbiAgICAgIHBsdWdpbnM6IFt0YWlsd2luZGNzcygpXSxcclxuICAgIH0sXHJcbiAgfSxcclxuICBwbHVnaW5zOiBbXHJcbiAgICByZWFjdCgpLFxyXG4gICAgc3Zncih7XHJcbiAgICAgIGluY2x1ZGU6IFwiKiovKi5zdmc/cmVhY3RcIixcclxuICAgIH0pLFxyXG4gICAgVGFuU3RhY2tSb3V0ZXJWaXRlKCksXHJcbiAgICBWYWxpZGF0ZUVudigpLFxyXG4gIF0sXHJcbiAgc2VydmVyOiB7XHJcbiAgICBob3N0OiBwcm9jZXNzLmVudi5WSVRFX0hPU1QsXHJcbiAgICBwb3J0OiA1MTczLFxyXG4gICAgc3RyaWN0UG9ydDogdHJ1ZSxcclxuICAgIGh0dHBzOiB7XHJcbiAgICAgIGtleTogZnMucmVhZEZpbGVTeW5jKHBhdGgucmVzb2x2ZShfX2Rpcm5hbWUsIFwiLi8uY2VydC9jZXJ0LmtleVwiKSksXHJcbiAgICAgIGNlcnQ6IGZzLnJlYWRGaWxlU3luYyhwYXRoLnJlc29sdmUoX19kaXJuYW1lLCBcIi4vLmNlcnQvY2VydC5jcnRcIikpLFxyXG4gICAgfSxcclxuICB9LFxyXG59KTtcclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUFzUSxPQUFPLFFBQVE7QUFDclIsT0FBTyxVQUFVO0FBQ2pCLFNBQVMsbUJBQW1CO0FBQzVCLFNBQVMsMEJBQTBCO0FBQ25DLE9BQU8sV0FBVztBQUNsQixPQUFPLFlBQVk7QUFDbkIsT0FBTyxpQkFBaUI7QUFDeEIsU0FBUyxvQkFBb0I7QUFDN0IsT0FBTyxVQUFVO0FBUmpCLElBQU0sbUNBQW1DO0FBVXpDLE9BQU8sT0FBTztBQUVkLElBQU8sc0JBQVEsYUFBYTtBQUFBLEVBQzFCLFNBQVM7QUFBQSxJQUNQLE9BQU87QUFBQSxNQUNMLEtBQUssS0FBSyxRQUFRLGtDQUFXLE9BQU87QUFBQSxJQUN0QztBQUFBLEVBQ0Y7QUFBQSxFQUNBLEtBQUs7QUFBQSxJQUNILFNBQVM7QUFBQSxNQUNQLFNBQVMsQ0FBQyxZQUFZLENBQUM7QUFBQSxJQUN6QjtBQUFBLEVBQ0Y7QUFBQSxFQUNBLFNBQVM7QUFBQSxJQUNQLE1BQU07QUFBQSxJQUNOLEtBQUs7QUFBQSxNQUNILFNBQVM7QUFBQSxJQUNYLENBQUM7QUFBQSxJQUNELG1CQUFtQjtBQUFBLElBQ25CLFlBQVk7QUFBQSxFQUNkO0FBQUEsRUFDQSxRQUFRO0FBQUEsSUFDTixNQUFNLFFBQVEsSUFBSTtBQUFBLElBQ2xCLE1BQU07QUFBQSxJQUNOLFlBQVk7QUFBQSxJQUNaLE9BQU87QUFBQSxNQUNMLEtBQUssR0FBRyxhQUFhLEtBQUssUUFBUSxrQ0FBVyxrQkFBa0IsQ0FBQztBQUFBLE1BQ2hFLE1BQU0sR0FBRyxhQUFhLEtBQUssUUFBUSxrQ0FBVyxrQkFBa0IsQ0FBQztBQUFBLElBQ25FO0FBQUEsRUFDRjtBQUNGLENBQUM7IiwKICAibmFtZXMiOiBbXQp9Cg==
