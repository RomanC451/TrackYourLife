import "./index.css";

import React from "react";
import { createRoot } from "react-dom/client";

import App from "./App";
import { ApiContextProvider } from "./contexts/ApiContextProvider";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";

const rootElement = document.getElementById("app")!;
const root = createRoot(rootElement);
root.render(
  // <React.StrictMode>
  <ApiContextProvider>
    <AppGeneralContextProvider>
      <App />
    </AppGeneralContextProvider>
  </ApiContextProvider>
  // </React.StrictMode>
);
