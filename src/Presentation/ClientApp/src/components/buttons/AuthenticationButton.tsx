import React from "react";

import "./AuthenticationButton.css";

interface AuthenticationButtonProps {
  className?: string;
  text?: string;
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const AuthenticationButton: React.FC<AuthenticationButtonProps> = ({
  className,
  text,
  onClick,
}) => {
  return (
    <button
      type="button"
      className={`${className} button-background  h-[50px] w-[80%] rounded-3xl text-white shadow-lg hover:shadow-2xl`}
      onClick={onClick}
    >
      {text}
    </button>
  );
};

export default AuthenticationButton;
