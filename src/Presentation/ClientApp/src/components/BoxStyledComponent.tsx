import { clsx } from "clsx";
import React from "react";
import { tailwindBackgroundColors } from "~/constants/tailwindColors";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

interface IProps {
  width: number;
  height: number;
  title: string;
  className?: string;
  bgColor?: string;
  borderRadius?: number;
  children: React.ReactNode;
}

const defaultBgColor = tailwindBackgroundColors["second-gray-bg"];
const defaultBorderRadius = 10;

const BoxStyledComponent: React.FC<IProps> = ({
  width,
  height,
  title,
  className = "",
  bgColor = defaultBgColor,
  borderRadius = defaultBorderRadius,
  children
}): JSX.Element => {
  return (
    // <ComponentTopBarMenuLayout title={title}>
    <div
      style={{
        minWidth: width,
        minHeight: height,
        backgroundColor: bgColor,
        borderRadius: borderRadius
      }}
      className={clsx(className, "relative border-[1px] border-border-gray")}
    >
      {children}
    </div>
    // </ComponentTopBarMenuLayout>
  );
};

export default BoxStyledComponent;
