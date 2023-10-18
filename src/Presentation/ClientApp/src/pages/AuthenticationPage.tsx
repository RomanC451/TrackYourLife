import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import {
  AuthenticationContextProvider,
  useAuthenticationContext
} from "~/contexts/authentication/AuthenticationContextProvider";
import { LogInForm, SignUpForm } from "~/features/authentication/";
import { authAlerts } from "~/features/authentication/data/alerts";
import { authModes } from "~/features/authentication/data/enums";

import { Alert, Grow } from "@mui/material";

import AuthCard from "../components/cards/AuthCard";

const AuthenticationPage = () => {
  return (
    <AuthenticationContextProvider>
      <Page />
    </AuthenticationContextProvider>
  );
};

export default AuthenticationPage;

/**
 * React component for the authentication page.
 * @returns A JSX Element.
 */
const Page = () => {
  const { screenSize } = useAppGeneralStateContext();
  const { authMode, alert, setAlert } = useAuthenticationContext();

  return (
    <main className=" flex min-h-[100vh] w-full items-center justify-center flex-col bg-white">
      <div className="top-[-82px] ">
        <div className="h-[62px] w-[400px] mb-[20px] grid place-content-center">
          {screenSize < screensEnum.lg && alert.message != "" ? (
            <Grow in={alert.message != ""} timeout={1000}>
              <Alert
                severity={alert.severity}
                onClose={() => setAlert(authAlerts.unknown)}
              >
                {alert.message}
              </Alert>
            </Grow>
          ) : null}
        </div>
        <div className="h-[600px]  rounded-2xl shadow-2xl shadow-slate-700 grid place-items-center ">
          <div className="relative  w-[400px] lg:w-[800px] flex h-[95%] items-center justify-around  rounded-2xl   border-1 border-gray-200 bg-white shadow-xl ">
            {screenSize >= screensEnum.lg ? (
              <>
                <AuthCard />
                <SignUpForm /> <LogInForm />
              </>
            ) : authMode === authModes.singUp ? (
              <SignUpForm />
            ) : (
              <LogInForm />
            )}
          </div>
        </div>
        <div className="h-[62px] mt-[20px]" />
      </div>
    </main>
  );
};
