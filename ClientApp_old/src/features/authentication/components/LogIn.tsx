import { CardDescription, CardTitle } from "~/chadcn/ui/card";
import { Separator } from "~/chadcn/ui/separator";
import { cn } from "~/utils";

import LogInForm from "./LogInForm";

type LoginProps = {
  className?: string;
  visible: boolean;
  switchToSignUp: () => void;
  isAnimating: boolean;
};

const LogIn: React.FC<LoginProps> = ({
  className,
  visible,
  switchToSignUp,
  isAnimating,
}) => {
  return (
    <section
      className={cn("flex flex-col items-center gap-2", className)}
      style={{ display: visible ? "flex" : "none" }}
    >
      <CardTitle>Log In</CardTitle>
      <CardDescription> Take control of your life</CardDescription>
      <Separator className="my-4" />
      <LogInForm switchToSignUp={switchToSignUp} isAnimating={isAnimating} />
    </section>
  );
};

export default LogIn;
