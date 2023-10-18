import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import {
  AuthenticationPage,
  EmailVerificationPage,
  HomePage,
  LandingPage,
  TestPage
} from "./pages";

const App: React.FC = (): JSX.Element => {
  return (
    <div className="w-full h-full">
      <BrowserRouter>
        <Routes>
          <Route path="/landing" element={<LandingPage />} />
          <Route path="/home" element={<HomePage />} />
          <Route path="/auth" element={<AuthenticationPage />} />
          <Route path="/asdsa" element={<TestPage />} />
          <Route
            path="/email-verification"
            element={<EmailVerificationPage />}
          />
        </Routes>
      </BrowserRouter>
    </div>
  );
};

export default App;
