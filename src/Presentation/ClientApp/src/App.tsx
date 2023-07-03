import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import { AuthenticationPage, HomePage, LandingPage } from "./pages";

const App: React.FC = (): JSX.Element => {
  console.log("rendering");
  return (
    <div className="w-full h-[100vh]">
      <BrowserRouter>
        <Routes>
          <Route path="/asdasd" element={<LandingPage />} />
          <Route path="/home" element={<HomePage />} />
          <Route path="/" element={<AuthenticationPage />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
};

export default App;
