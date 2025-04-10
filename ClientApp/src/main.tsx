
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";

import App from "./App.tsx";

import "@/services/openapi/retryOnUnauthorizedInterceptor.ts";
import "./index.css";

// scan({
//   enabled: true,
//   log: true, // logs render info to console (default: false)
// });

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
