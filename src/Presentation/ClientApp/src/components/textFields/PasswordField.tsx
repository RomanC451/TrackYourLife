import React, { useState } from "react";
import { UseFormRegisterReturn } from "react-hook-form";

import { Visibility, VisibilityOff } from "@mui/icons-material";
import {
  FormControl,
  FormHelperText,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput
} from "@mui/material";

interface PasswordFieldProps {
  label: string;
  className?: string;
  id?: number;
  errorMessage?: string;
  inputProps?: UseFormRegisterReturn;
  defaultValue?: string;
}

const PasswordField: React.FC<PasswordFieldProps> = ({
  label,
  className,
  id,
  errorMessage = "",
  inputProps,
  defaultValue
}) => {
  const [showPassword, setShowPassword] = useState(false);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (
    event: React.MouseEvent<HTMLButtonElement, MouseEvent>
  ): void => {
    event.preventDefault();
  };

  return (
    <div style={{ paddingBottom: errorMessage ? 0 : 23 }} className={className}>
      <FormControl variant="outlined" fullWidth>
        <InputLabel
          error={errorMessage != ""}
          htmlFor="outlined-adornment-password"
        >
          {label}
        </InputLabel>
        <OutlinedInput
          {...inputProps}
          defaultValue={defaultValue}
          error={errorMessage != ""}
          type={showPassword ? "text" : "password"}
          inputProps={{ "data-testid": `id-input-${id}` }}
          autoComplete=""
          endAdornment={
            <InputAdornment position="end" className="iconButton">
              <IconButton
                className="iconButton"
                aria-label="toggle password visibility"
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                {showPassword ? (
                  <VisibilityOff className="iconButton" />
                ) : (
                  <Visibility className="iconButton" />
                )}
              </IconButton>
            </InputAdornment>
          }
          label={label}
        />
        {errorMessage != "" ? (
          <FormHelperText error id="outlined-weight-helper-text">
            {errorMessage}
          </FormHelperText>
        ) : null}
      </FormControl>
    </div>
  );
};

export default PasswordField;
