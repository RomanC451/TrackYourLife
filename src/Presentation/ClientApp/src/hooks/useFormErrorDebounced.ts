import { Timeout } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { FieldError } from "react-hook-form";

type UseFormErrorDebouncedProps = {
  fieldError: FieldError | undefined;
  isSubmitting: boolean;
};

const useFormErrorDebounced = ({
  fieldError,
  isSubmitting,
}: UseFormErrorDebouncedProps) => {
  const [error, setError] = useState<FieldError | undefined>(undefined);

  useEffect(() => {
    let timeoutId: Timeout | undefined;

    if (!isSubmitting) {
      if (error === undefined) {
        setError(fieldError);
      } else {
        timeoutId = setTimeout(() => {
          setError(fieldError);
        }, 500);
      }
    }

    return () => {
      if (timeoutId) {
        clearTimeout(timeoutId);
      }
    };
  }, [fieldError, isSubmitting]);

  return [error];
};

export default useFormErrorDebounced;
