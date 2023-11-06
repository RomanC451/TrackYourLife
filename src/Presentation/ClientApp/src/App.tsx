import React, { useEffect } from "react";
import { Route, Routes } from "react-router-dom";
import RequireAuth from "~/auth/RequireAuth";
import useUserData from "~/auth/useUserData";
import { userDataInitValue } from "~/contexts/AuthenticationContextProvider";
import MissingPage from "~/pages/MissingPage";

import {
  AuthenticationPage,
  EmailVerificationPage,
  HealthPage,
  LandingPage,
  TestPage
} from "./pages";

const App: React.FC = (): JSX.Element => {
  return (
    <Routes>
      <Route path="/landing" element={<LandingPage />} />
      <Route path="/auth" element={<AuthenticationPage />} />
      <Route path="/test" element={<TestPage />} />
      <Route path="/email-verification" element={<EmailVerificationPage />} />
      <Route element={<RequireAuth />}>
        <Route path="/health" element={<HealthPage />} />
      </Route>

      <Route path="*" element={<MissingPage />} />
    </Routes>
  );
};

export default App;
