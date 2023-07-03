import React from "react";

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
  name: string;
  className?: string;
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
  errorMessage?: string;
}

const CustomTextField: React.FC<TextFieldProps> = ({
  label,
  name,
  className,
  onChange,
  errorMessage = ""
}) => {
  return (
    <div className={className}>
      <TextField
        fullWidth
        error={errorMessage != ""}
        label={label}
        name={name}
        helperText={errorMessage}
        onChange={onChange}
      />
    </div>
  );
};

export default CustomTextField;
