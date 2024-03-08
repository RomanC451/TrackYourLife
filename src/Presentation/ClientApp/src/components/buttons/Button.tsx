import React from "react";

interface ButtonInterface {
  Icon?: React.ReactNode;
  bgColor?: string;
  color?: string;
  bgHoverColor?: string;
  size?: string;
  text?: string;
  borderRadius?: string;
  width?: number;
  onClick: React.MouseEventHandler;
  className?: string;
}

const Button: React.FC<ButtonInterface> = ({
  Icon,
  bgColor,
  color,
  bgHoverColor,
  size,
  text,
  borderRadius,
  width,
  onClick,
  className,
}): JSX.Element => {
  return (
    <button
      type="button"
      onClick={onClick}
      style={{ backgroundColor: bgColor, color, borderRadius }}
      className={` text-${size} p-3 w-${width} hover:drop-shadow-xl hover:bg-${bgHoverColor} ${className}`}
    >
      {Icon}
      <p> {text}</p>
    </button>
  );
};

export default Button;
