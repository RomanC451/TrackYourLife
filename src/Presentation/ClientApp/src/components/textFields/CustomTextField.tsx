import React from "react";
import { UseFormRegisterReturn } from "react-hook-form";

import { TextField } from "@mui/material";

/**
 * A functional component for text input field with label and change event handling.
 * @param props - The properties of the component.
 * @param props.label - The label of the text input field.
 * @param  props.className - The className to be applied to the outer div.
 * @param props.onChange - The function to be called on change event of the input field.
 * @returns a text input field with label and change event handling.
 */

interface TextFieldProps {
  label: string;
  className?: string;
  errorMessage: string | undefined;
  inputProps?: Omit<UseFormRegisterReturn, "ref">;
  defaultValue?: string;
}

const CustomTextField: React.FC<TextFieldProps> = ({
  label,
  className,
  errorMessage,
  inputProps,
  defaultValue
}) => {
  return (
    <div style={{ paddingBottom: errorMessage ? 0 : 23 }} className={className}>
      <TextField
        fullWidth
        error={!!errorMessage}
        label={label}
        helperText={errorMessage}
        {...inputProps}
        defaultValue={defaultValue}
      />
    </div>
  );
};

export default CustomTextField;
