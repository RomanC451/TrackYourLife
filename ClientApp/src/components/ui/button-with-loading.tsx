import React from "react";
import CircularProgress from "@mui/material/CircularProgress/CircularProgress";
import { VariantProps } from "class-variance-authority";

import { Button, buttonVariants } from "./button";

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
  isLoading?: boolean;
}

const ButtonWithLoading = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ isLoading, children, ...props }, ref) => {
    return (
      <Button ref={ref} {...props} className="[&_svg]:size-auto">
        <div className="relative">
          {isLoading ? (
            <CircularProgress size={20} className="absolute -left-8" />
          ) : null}
          {children}
        </div>
      </Button>
    );
  },
);

export default ButtonWithLoading;
