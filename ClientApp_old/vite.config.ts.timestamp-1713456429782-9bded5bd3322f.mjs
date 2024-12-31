// vite.config.ts
import { TanStackRouterVite } from "file:///E:/TrackYourLifeDotnet/src/Presentation/ClientApp/node_modules/@tanstack/router-vite-plugin/dist/esm/index.js";
import react from "file:///E:/TrackYourLifeDotnet/src/Presentation/ClientApp/node_modules/@vitejs/plugin-react-swc/index.mjs";
import path from "path";
import { defineConfig } from "file:///E:/TrackYourLifeDotnet/src/Presentation/ClientApp/node_modules/vite/dist/node/index.js";
import mkcert from "file:///E:/TrackYourLifeDotnet/src/Presentation/ClientApp/node_modules/vite-plugin-mkcert/dist/mkcert.mjs";
import svgr from "file:///E:/TrackYourLifeDotnet/src/Presentation/ClientApp/node_modules/vite-plugin-svgr/dist/index.js";
var __vite_injected_original_dirname = "E:\\TrackYourLifeDotnet\\src\\Presentation\\ClientApp";
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
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJFOlxcXFxUcmFja1lvdXJMaWZlRG90bmV0XFxcXHNyY1xcXFxQcmVzZW50YXRpb25cXFxcQ2xpZW50QXBwXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJFOlxcXFxUcmFja1lvdXJMaWZlRG90bmV0XFxcXHNyY1xcXFxQcmVzZW50YXRpb25cXFxcQ2xpZW50QXBwXFxcXHZpdGUuY29uZmlnLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9FOi9UcmFja1lvdXJMaWZlRG90bmV0L3NyYy9QcmVzZW50YXRpb24vQ2xpZW50QXBwL3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHsgVGFuU3RhY2tSb3V0ZXJWaXRlIH0gZnJvbSBcIkB0YW5zdGFjay9yb3V0ZXItdml0ZS1wbHVnaW5cIjtcbmltcG9ydCByZWFjdCBmcm9tIFwiQHZpdGVqcy9wbHVnaW4tcmVhY3Qtc3djXCI7XG5pbXBvcnQgcGF0aCBmcm9tIFwicGF0aFwiO1xuaW1wb3J0IHsgZGVmaW5lQ29uZmlnIH0gZnJvbSBcInZpdGVcIjtcbmltcG9ydCBta2NlcnQgZnJvbSBcInZpdGUtcGx1Z2luLW1rY2VydFwiO1xuaW1wb3J0IHN2Z3IgZnJvbSBcInZpdGUtcGx1Z2luLXN2Z3JcIjtcblxuLy8gaHR0cHM6Ly92aXRlanMuZGV2L2NvbmZpZy9cbmV4cG9ydCBkZWZhdWx0IGRlZmluZUNvbmZpZyh7XG4gIHJlc29sdmU6IHtcbiAgICBhbGlhczoge1xuICAgICAgXCJ+XCI6IHBhdGgucmVzb2x2ZShfX2Rpcm5hbWUsIFwiLi9zcmNcIiksXG4gICAgfSxcbiAgfSxcbiAgcGx1Z2luczogW1xuICAgIHJlYWN0KCksXG4gICAgc3Zncih7XG4gICAgICAvLyBzdmdyT3B0aW9uczoge1xuICAgICAgLy8gICBleHBvcnRUeXBlOiBcIm5hbWVkXCIsXG4gICAgICAvLyAgIHJlZjogdHJ1ZSxcbiAgICAgIC8vICAgc3ZnbzogZmFsc2UsXG4gICAgICAvLyAgIHRpdGxlUHJvcDogdHJ1ZSxcbiAgICAgIC8vIH0sXG4gICAgICBpbmNsdWRlOiBcIioqLyouc3ZnP3JlYWN0XCIsXG4gICAgfSksXG4gICAgVGFuU3RhY2tSb3V0ZXJWaXRlKCksXG5cbiAgICBta2NlcnQoeyBob3N0czogW1wiaHR0cHM6Ly8xOTIuMTY4LjEuNjo1MTczL1wiXSB9KSxcbiAgICAvLyBiYXNpY1NzbCh7IGRvbWFpbnM6IFtcImh0dHBzOi8vMTkyLjE2OC4xLjg6NTE3My9cIl0sIG5hbWU6IFwiVHJhY2tZb3VyTGlmZVwiIH0pLFxuICBdLFxuICBzZXJ2ZXI6IHtcbiAgICBob3N0OiB0cnVlLFxuICAgIHBvcnQ6IDUxNzMsXG4gICAgc3RyaWN0UG9ydDogdHJ1ZSxcbiAgfSxcbn0pO1xuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUErVSxTQUFTLDBCQUEwQjtBQUNsWCxPQUFPLFdBQVc7QUFDbEIsT0FBTyxVQUFVO0FBQ2pCLFNBQVMsb0JBQW9CO0FBQzdCLE9BQU8sWUFBWTtBQUNuQixPQUFPLFVBQVU7QUFMakIsSUFBTSxtQ0FBbUM7QUFRekMsSUFBTyxzQkFBUSxhQUFhO0FBQUEsRUFDMUIsU0FBUztBQUFBLElBQ1AsT0FBTztBQUFBLE1BQ0wsS0FBSyxLQUFLLFFBQVEsa0NBQVcsT0FBTztBQUFBLElBQ3RDO0FBQUEsRUFDRjtBQUFBLEVBQ0EsU0FBUztBQUFBLElBQ1AsTUFBTTtBQUFBLElBQ04sS0FBSztBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBLE1BT0gsU0FBUztBQUFBLElBQ1gsQ0FBQztBQUFBLElBQ0QsbUJBQW1CO0FBQUEsSUFFbkIsT0FBTyxFQUFFLE9BQU8sQ0FBQywyQkFBMkIsRUFBRSxDQUFDO0FBQUE7QUFBQSxFQUVqRDtBQUFBLEVBQ0EsUUFBUTtBQUFBLElBQ04sTUFBTTtBQUFBLElBQ04sTUFBTTtBQUFBLElBQ04sWUFBWTtBQUFBLEVBQ2Q7QUFDRixDQUFDOyIsCiAgIm5hbWVzIjogW10KfQo=
