import React from "react";
import { VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

import { Button, buttonVariants } from "./button";
import Spinner from "./spinner";

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
  isLoading: boolean;
}

/// !! TO DO: pass is pending instead of isLoading and disabled. And check the status here.
const ButtonWithLoading = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ isLoading, children, className, ...props }, ref) => {
    return (
      <Button ref={ref} {...props} className={cn(className)}>
        {isLoading ? <Spinner /> : null}
        {children}
      </Button>
    );
  },
);

export default ButtonWithLoading;
