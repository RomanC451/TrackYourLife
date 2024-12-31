// vite.config.ts
import { TanStackRouterVite } from "file:///D:/TrackYourLife/Common/TrackYourLife.Common.Presentation/ClientApp/node_modules/@tanstack/router-vite-plugin/dist/esm/index.js";
import react from "file:///D:/TrackYourLife/Common/TrackYourLife.Common.Presentation/ClientApp/node_modules/@vitejs/plugin-react-swc/index.mjs";
import path from "path";
import { defineConfig } from "file:///D:/TrackYourLife/Common/TrackYourLife.Common.Presentation/ClientApp/node_modules/vite/dist/node/index.js";
import mkcert from "file:///D:/TrackYourLife/Common/TrackYourLife.Common.Presentation/ClientApp/node_modules/vite-plugin-mkcert/dist/mkcert.mjs";
import svgr from "file:///D:/TrackYourLife/Common/TrackYourLife.Common.Presentation/ClientApp/node_modules/vite-plugin-svgr/dist/index.js";
var __vite_injected_original_dirname = "D:\\TrackYourLife\\Common\\TrackYourLife.Common.Presentation\\ClientApp";
var vite_config_default = defineConfig({
  resolve: {
    alias: {
      "~": path.resolve(__vite_injected_original_dirname, "./src")
    }
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
      include: "**/*.svg?react"
    }),
    TanStackRouterVite(),
    mkcert({ hosts: ["https://192.168.1.6:5173/"] })
    // basicSsl({ domains: ["https://192.168.1.8:5173/"], name: "TrackYourLife" }),
  ],
  server: {
    host: true,
    port: 5173,
    strictPort: true
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJEOlxcXFxUcmFja1lvdXJMaWZlXFxcXENvbW1vblxcXFxUcmFja1lvdXJMaWZlLkNvbW1vbi5QcmVzZW50YXRpb25cXFxcQ2xpZW50QXBwXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJEOlxcXFxUcmFja1lvdXJMaWZlXFxcXENvbW1vblxcXFxUcmFja1lvdXJMaWZlLkNvbW1vbi5QcmVzZW50YXRpb25cXFxcQ2xpZW50QXBwXFxcXHZpdGUuY29uZmlnLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9EOi9UcmFja1lvdXJMaWZlL0NvbW1vbi9UcmFja1lvdXJMaWZlLkNvbW1vbi5QcmVzZW50YXRpb24vQ2xpZW50QXBwL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHsgVGFuU3RhY2tSb3V0ZXJWaXRlIH0gZnJvbSBcIkB0YW5zdGFjay9yb3V0ZXItdml0ZS1wbHVnaW5cIjtcclxuaW1wb3J0IHJlYWN0IGZyb20gXCJAdml0ZWpzL3BsdWdpbi1yZWFjdC1zd2NcIjtcclxuaW1wb3J0IHBhdGggZnJvbSBcInBhdGhcIjtcclxuaW1wb3J0IHsgZGVmaW5lQ29uZmlnIH0gZnJvbSBcInZpdGVcIjtcclxuaW1wb3J0IG1rY2VydCBmcm9tIFwidml0ZS1wbHVnaW4tbWtjZXJ0XCI7XHJcbmltcG9ydCBzdmdyIGZyb20gXCJ2aXRlLXBsdWdpbi1zdmdyXCI7XHJcblxyXG4vLyBodHRwczovL3ZpdGVqcy5kZXYvY29uZmlnL1xyXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xyXG4gIHJlc29sdmU6IHtcclxuICAgIGFsaWFzOiB7XHJcbiAgICAgIFwiflwiOiBwYXRoLnJlc29sdmUoX19kaXJuYW1lLCBcIi4vc3JjXCIpLFxyXG4gICAgfSxcclxuICB9LFxyXG4gIHBsdWdpbnM6IFtcclxuICAgIHJlYWN0KCksXHJcbiAgICBzdmdyKHtcclxuICAgICAgLy8gc3Znck9wdGlvbnM6IHtcclxuICAgICAgLy8gICBleHBvcnRUeXBlOiBcIm5hbWVkXCIsXHJcbiAgICAgIC8vICAgcmVmOiB0cnVlLFxyXG4gICAgICAvLyAgIHN2Z286IGZhbHNlLFxyXG4gICAgICAvLyAgIHRpdGxlUHJvcDogdHJ1ZSxcclxuICAgICAgLy8gfSxcclxuICAgICAgaW5jbHVkZTogXCIqKi8qLnN2Zz9yZWFjdFwiLFxyXG4gICAgfSksXHJcbiAgICBUYW5TdGFja1JvdXRlclZpdGUoKSxcclxuXHJcbiAgICBta2NlcnQoeyBob3N0czogW1wiaHR0cHM6Ly8xOTIuMTY4LjEuNjo1MTczL1wiXSB9KSxcclxuICAgIC8vIGJhc2ljU3NsKHsgZG9tYWluczogW1wiaHR0cHM6Ly8xOTIuMTY4LjEuODo1MTczL1wiXSwgbmFtZTogXCJUcmFja1lvdXJMaWZlXCIgfSksXHJcbiAgXSxcclxuICBzZXJ2ZXI6IHtcclxuICAgIGhvc3Q6IHRydWUsXHJcbiAgICBwb3J0OiA1MTczLFxyXG4gICAgc3RyaWN0UG9ydDogdHJ1ZSxcclxuICB9LFxyXG59KTtcclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUFxWSxTQUFTLDBCQUEwQjtBQUN4YSxPQUFPLFdBQVc7QUFDbEIsT0FBTyxVQUFVO0FBQ2pCLFNBQVMsb0JBQW9CO0FBQzdCLE9BQU8sWUFBWTtBQUNuQixPQUFPLFVBQVU7QUFMakIsSUFBTSxtQ0FBbUM7QUFRekMsSUFBTyxzQkFBUSxhQUFhO0FBQUEsRUFDMUIsU0FBUztBQUFBLElBQ1AsT0FBTztBQUFBLE1BQ0wsS0FBSyxLQUFLLFFBQVEsa0NBQVcsT0FBTztBQUFBLElBQ3RDO0FBQUEsRUFDRjtBQUFBLEVBQ0EsU0FBUztBQUFBLElBQ1AsTUFBTTtBQUFBLElBQ04sS0FBSztBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBLE1BT0gsU0FBUztBQUFBLElBQ1gsQ0FBQztBQUFBLElBQ0QsbUJBQW1CO0FBQUEsSUFFbkIsT0FBTyxFQUFFLE9BQU8sQ0FBQywyQkFBMkIsRUFBRSxDQUFDO0FBQUE7QUFBQSxFQUVqRDtBQUFBLEVBQ0EsUUFBUTtBQUFBLElBQ04sTUFBTTtBQUFBLElBQ04sTUFBTTtBQUFBLElBQ04sWUFBWTtBQUFBLEVBQ2Q7QUFDRixDQUFDOyIsCiAgIm5hbWVzIjogW10KfQo=
