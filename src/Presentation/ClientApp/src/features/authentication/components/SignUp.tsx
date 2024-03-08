import React from "react";

import { CardDescription, CardTitle } from "~/chadcn/ui/card";
import { Separator } from "~/chadcn/ui/separator";
import { cn } from "~/utils";

import { SignUpForm } from "..";

type SignUpProps = {
  className?: string;
  visible: boolean;
  swithcToLogIn: () => void;
  isAnimating: boolean;
};
const SignUp: React.FC<SignUpProps> = ({
  className,
  visible,
  swithcToLogIn,
  isAnimating,
}) => {
  return (
    <section
      className={cn("flex flex-col items-center gap-2", className)}
      style={{ display: visible ? "flex" : "none" }}
    >
      <CardTitle>Sign Up</CardTitle>
      <CardDescription> Take control of your life</CardDescription>
      <Separator className="my-4" />
      <SignUpForm swithcToLogIn={swithcToLogIn} isAnimating={isAnimating} />
    </section>
  );
};

export default SignUp;
