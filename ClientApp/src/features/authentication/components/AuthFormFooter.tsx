import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { MutationPendingState } from "@/hooks/useCustomMutation";

import { AuthMode, authModes } from "../data/enums";
import ThirdAppsAuth from "./ThirdAppsAuth";

type AuthFormFooterProps = {
  disabled: boolean;
  pendingState: MutationPendingState;
  switchToSignUp: () => void;
  authMode: AuthMode;
};

const AuthFormFooter = ({
  disabled,
  pendingState,
  switchToSignUp,
  authMode,
}: AuthFormFooterProps) => {
  return (
    <>
      <ButtonWithLoading
        type="submit"
        disabled={disabled || pendingState.isPending}
        isLoading={pendingState.isDelayedPending}
      >
        <span>{authMode === authModes.logIn ? "Log In" : "Sign Up"}</span>
      </ButtonWithLoading>
      <ThirdAppsAuth
        disabled={disabled || pendingState.isPending}
        authMode={authMode}
      />
      <Button
        type="button"
        variant="ghost"
        onClick={switchToSignUp}
        disabled={disabled || pendingState.isPending}
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
