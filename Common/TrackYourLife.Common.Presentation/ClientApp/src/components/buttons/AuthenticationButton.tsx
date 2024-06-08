import "./AuthenticationButton.css";

import React from "react";

interface AuthenticationButtonProps {
  className?: string;
  text?: string;
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const AuthenticationButton: React.FC<AuthenticationButtonProps> = ({
  className,
  text,
  onClick
}) => {
  return (
    <button
      type="submit"
      className={`${className} bg-gray  h-[50px] w-[80%] rounded-3xl text-white shadow-lg hover:shadow-2xl`}
      onClick={onClick}
    >
      {text}
    </button>
  );
};

export default AuthenticationButton;
