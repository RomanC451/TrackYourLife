import React, { useState } from "react";

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
  name: string;
  className?: string;
  id?: number;
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onKeyPress?: (event: React.KeyboardEvent<HTMLInputElement>) => void;
  errorMessage?: string;
}

const PasswordField: React.FC<PasswordFieldProps> = ({
  label,
  name,
  className,
  id,
  onChange,
  onKeyPress = () => {},
  errorMessage = ""
}) => {
  const [showPassword, setShowPassword] = useState(false);

  const handleClickShowPassword = () => setShowPassword((show) => !show);

  const handleMouseDownPassword = (
    event: React.MouseEvent<HTMLButtonElement, MouseEvent>
  ): void => {
    event.preventDefault();
  };

  return (
    <div className={className}>
      <FormControl variant="outlined" fullWidth>
        <InputLabel
          error={errorMessage != ""}
          htmlFor="outlined-adornment-password"
        >
          {label}
        </InputLabel>
        <OutlinedInput
          error={errorMessage != ""}
          name={name}
          type={showPassword ? "text" : "password"}
          inputProps={{ "data-testid": `id-input-${id}` }}
          onChange={onChange}
          onKeyPress={onKeyPress}
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
