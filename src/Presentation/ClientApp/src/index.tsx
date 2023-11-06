import "./index.css";

import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { AuthenticationContextProvider } from "~/contexts/AuthenticationContextProvider";

import App from "./App";
import { ApiContextProvider } from "./contexts/ApiContextProvider";
import { AppGeneralContextProvider } from "./contexts/AppGeneralContextProvider";

const rootElement = document.getElementById("app")!;
const root = createRoot(rootElement);
root.render(
  // <React.StrictMode>
  <ApiContextProvider>
    <AuthenticationContextProvider>
      <AppGeneralContextProvider>
        <div className="w-full h-full">
          <BrowserRouter>
            <App />
          </BrowserRouter>
        </div>
      </AppGeneralContextProvider>
    </AuthenticationContextProvider>
  </ApiContextProvider>
  // </React.StrictMode>
);
