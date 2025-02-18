import React from "react";
import CircularProgress from "@mui/material/CircularProgress/CircularProgress";
import { VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

import { Button, buttonVariants } from "./button";

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
  isLoading: boolean;
}

const ButtonWithLoading = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ isLoading, children, className, ...props }, ref) => {
    return (
      <Button
        ref={ref}
        {...props}
        className={cn("[&_svg]:size-auto", className)}
      >
        <div className="relative h-full w-full">
          {isLoading ? (
            <div className="absolute left-[50%] top-[50%] size-[20px] translate-x-[-50%] translate-y-[-50%]">
              <CircularProgress color="inherit" size={20} />
            </div>
          ) : (
            <div className="absolute left-[50%] top-[50%] translate-x-[-50%] translate-y-[-50%]">
              {children}
            </div>
          )}
          <div className="opacity-0">{children}</div>
        </div>
      </Button>
    );
  },
);

export default ButtonWithLoading;
