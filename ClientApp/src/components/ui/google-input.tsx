import React, { ChangeEvent, useState } from "react";
import { motion, Variants } from "framer-motion";

import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";

interface GoogleInputProps {
  label: string;
  id?: string;
  type?: string;
  error?: boolean;
  helperText?: string;
}

const GoogleInput = React.forwardRef<
  HTMLInputElement,
  React.ComponentProps<"input"> & GoogleInputProps
>(
  (
    {
      id,
      label,
      helperText,
      type = "text",
      error = false,
      onChange,
      onFocus,
      ...props
    },
    ref,
  ) => {
    const [isFocused, setIsFocused] = useState(false);
    const [inputValue, setInputValue] = useState("");

    const labelVariants: Variants = {
      default: { top: "12px", fontSize: "16px", color: "#999" },
      active: { top: "-10px", fontSize: "12px", color: "#4285f4" },
    };

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
      setInputValue(e.target.value);
      onChange?.(e);
    };

    return (
      <div className="center size-full w-full">
        <div className="relative mx-auto w-full">
          <Input
            ref={ref}
            type={type}
            id={id}
            className={cn(
              "h-12 w-full rounded border border-gray-300 p-3 text-base transition-colors focus:border-blue-500 focus:outline-none",
            )}
            value={inputValue}
            onChange={handleInputChange}
            onFocus={(e) => {
              setIsFocused(true);
              onFocus?.(e);
            }}
            onBlur={() => setIsFocused(false)}
            {...props}
          />
          <Label asChild>
            <motion.label
              htmlFor={id}
              className="pointer-events-none absolute left-3 bg-background p-1"
              initial="default"
              animate={isFocused || inputValue ? "active" : "default"}
              variants={labelVariants}
              transition={{ duration: 0.1 }}
            >
              {error ? <p className="text-red-500">{helperText}</p> : label}
            </motion.label>
          </Label>
        </div>
      </div>
    );
  },
);

export default GoogleInput;
