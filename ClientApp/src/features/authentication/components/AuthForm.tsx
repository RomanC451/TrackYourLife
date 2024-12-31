import { memo } from "react";

import { CardDescription, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";

import { AuthMode, authModes } from "../data/enums";
import LogInForm from "./LogInForm";
import SignUpForm from "./SignUpForm";

type AuthFormProps = {
  className?: string;
  switchAuthMode: () => void;
  disabled: boolean;
  visible: boolean;
  mode: AuthMode;
};

const AuthForm = memo(function ({
  className,
  switchAuthMode,
  disabled,
  visible,
  mode,
}: AuthFormProps) {
  return (
    <section
      className={cn(
        "inline-flex w-full flex-col items-center gap-2",
        visible ? "" : "hidden",
        className,
      )}
    >
      <CardTitle>{mode === authModes.logIn ? "Log In" : "Sign Up"}</CardTitle>
      <CardDescription> Take control of your life</CardDescription>
      <Separator className="my-4" />
      {mode === authModes.logIn ? (
        <LogInForm switchToSignUp={switchAuthMode} disabled={disabled} />
      ) : (
        <SignUpForm switchToLogIn={switchAuthMode} disabled={disabled} />
      )}
    </section>
  );
});

export default AuthForm;
