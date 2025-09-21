import { useCallback, useEffect, useState } from "react";
import { useLocalStorage } from "usehooks-ts";

import AuthNavBar from "@/components/navbar/AuthNavBar";
import { Card } from "@/components/ui/card";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import AuthCard from "@/features/authentication/components/AuthCard";
import AuthForm from "@/features/authentication/components/AuthForm";
import { AuthMode, authModes } from "@/features/authentication/data/enums";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";
import NavBarLayout from "@/layouts/NavBarLayout";

export const AuthPage = () => {
  const { screenSize } = useAppGeneralStateContext();

  const [authMode, setAuthMode] = useLocalStorage<AuthMode>(
    "authenticationMethod",
    authModes.logIn,
  );

  const [isAnimating, setIsAnimating] = useState(false);

  const [isFullView, setIsFullView] = useState(
    screenSize.width >= screensEnum.lg,
  );

  useEffect(() => {
    setIsFullView(screenSize.width >= screensEnum.lg);
  }, [screenSize]);

  const toggleAuthMode = useCallback(() => {
    setAuthMode(
      authMode === authModes.logIn ? authModes.singUp : authModes.logIn,
    );
  }, [authMode, setAuthMode]);

  const logInActive = isFullView || authMode === authModes.logIn;
  const signUpActive = isFullView || authMode === authModes.singUp;

  return (
    <NavBarLayout navBarElement={<AuthNavBar />}>
      <FullSizeCenteredLayout>
        <Card className="relative -mt-16 inline-flex h-[650px] w-full max-w-[800px] gap-10 border-0 p-[min(40px,10%)] shadow-none sm:w-[80%] sm:border sm:shadow-2xl">
          <AuthForm
            mode={authModes.singUp}
            visible={signUpActive}
            switchAuthMode={toggleAuthMode}
            disabled={isAnimating}
          />
          <AuthForm
            mode={authModes.logIn}
            visible={logInActive}
            switchAuthMode={toggleAuthMode}
            disabled={isAnimating}
          />
          <AuthCard
            authMode={authMode}
            visible={isFullView}
            onAnimationStart={() => setIsAnimating(true)}
            onAnimationComplete={() => setIsAnimating(false)}
          />
        </Card>
      </FullSizeCenteredLayout>
    </NavBarLayout>
  );
};

export default AuthPage;
