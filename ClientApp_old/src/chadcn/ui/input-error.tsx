import React from "react";
import { ApiError } from "~/data/apiSettings";
import { cn } from "~/utils";

type FormMessageProps = (
  | { error: ApiError | null; message?: never }
  | { error?: never; message: string }
) & { isError: boolean } & React.HTMLAttributes<HTMLParagraphElement>;

const InputError = React.forwardRef<HTMLParagraphElement, FormMessageProps>(
  ({ className, children, isError, ...props }, ref) => {
    const { error, message, ...restProps } = props;

    if (!isError) {
      return null;
    }

    return (
      <p
        ref={ref}
        className={cn("text-sm font-medium text-destructive", className)}
        {...restProps}
      >
        {error ? error.response?.data?.detail : message}
      </p>
    );
  },
);

export { InputError };
