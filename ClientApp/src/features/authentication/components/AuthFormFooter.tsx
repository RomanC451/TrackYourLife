import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { LoadingState } from "@/hooks/useDelayedLoading";

import { AuthMode, authModes } from "../data/enums";
import ThirdAppsAuth from "./ThirdAppsAuth";

type AuthFormFooterProps = {
  disabled: boolean;
  isSubmitting: LoadingState;
  switchToSignUp: () => void;
  authMode: AuthMode;
};

const AuthFormFooter = ({
  disabled,
  isSubmitting,
  switchToSignUp,
  authMode,
}: AuthFormFooterProps) => {
  return (
    <>
      <ButtonWithLoading
        type="submit"
        disabled={disabled || !isSubmitting.isLoaded}
        isLoading={isSubmitting.isLoading}
      >
        <span>{authMode === authModes.logIn ? "Log In" : "Sign Up"}</span>
      </ButtonWithLoading>
      <ThirdAppsAuth
        disabled={disabled || !isSubmitting.isLoaded}
        authMode={authMode}
      />
      <Button
        type="button"
        variant="ghost"
        onClick={switchToSignUp}
        disabled={disabled || !isSubmitting.isLoaded}
        className="underline"
      >
        {authMode === authModes.logIn
          ? "I don't have an account."
          : "I already have an account."}
      </Button>
    </>
  );
};

export default AuthFormFooter;
