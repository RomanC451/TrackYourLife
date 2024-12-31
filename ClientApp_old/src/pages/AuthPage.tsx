import { useState } from "react";
import { useLocalStorage } from "usehooks-ts";
import { Card } from "~/chadcn/ui/card";
import AuthNavBar from "~/components/navbar/AuthNavBar";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import { AuthCard } from "~/features/authentication";
import LogIn from "~/features/authentication/components/LogIn";
import SignUp from "~/features/authentication/components/SignUp";
import { cardTransitionProps } from "~/features/authentication/data/animationsConfig";
import {
  TAuthModes,
  authModesEnum,
} from "~/features/authentication/data/enums";
import FullSizeCenteredLayout from "~/layouts/FullSizeCenteredLayout";
import NavBarLayout from "~/layouts/NavBarLayout";
import RootLayout from "~/layouts/RootLayout";

const AuthPage = () => {
  return <Page />;
};

export default AuthPage;

/**
 * React component for the authentication page.
 * @returns A JSX Element.
 */
const Page = () => {
  const { screenSize } = useAppGeneralStateContext();

  const [authMode, setAuthMode] = useLocalStorage<TAuthModes>(
    "authenticationMethod",
    authModesEnum.logIn,
  );

  const [isAnimating, setIsAnimating] = useState(false);

  function switchAuthMode() {
    if (isAnimating) return;

    setAuthMode(
      authMode === authModesEnum.logIn
        ? authModesEnum.singUp
        : authModesEnum.logIn,
    );
    if (screenSize.width >= screensEnum.lg) {
      setIsAnimating(true);
      setTimeout(() => {
        setIsAnimating(false);
      }, cardTransitionProps.duration * 1000);
    }
  }

  return (
    <RootLayout>
      <NavBarLayout navBarElement={<AuthNavBar />}>
        <FullSizeCenteredLayout className="">
          <Card className="relative -mt-16 flex h-[650px] w-full max-w-[800px] gap-10 border-0 p-[min(40px,10%)] shadow-none sm:w-[80%] sm:border sm:shadow-sm">
            <LogIn
              className="hidden w-full flex-grow"
              visible={
                screenSize.width >= screensEnum.lg ||
                authMode === authModesEnum.logIn
              }
              switchToSignUp={switchAuthMode}
              isAnimating={isAnimating}
            />
            <SignUp
              className="hidden w-full flex-grow"
              visible={
                screenSize.width >= screensEnum.lg ||
                authMode === authModesEnum.singUp
              }
              switchToLogIn={switchAuthMode}
              isAnimating={isAnimating}
            />
            {screenSize.width >= screensEnum.lg ? (
              <AuthCard authMode={authMode} />
            ) : null}
          </Card>
        </FullSizeCenteredLayout>
      </NavBarLayout>
    </RootLayout>
  );
};
