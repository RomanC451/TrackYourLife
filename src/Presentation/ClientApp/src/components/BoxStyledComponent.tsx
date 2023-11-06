import { clsx } from "clsx";
import React from "react";
import { tailwindBackgroundColors } from "~/constants/tailwindColors";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

interface IProps {
  minWidth: number;
  height: number;
  title: string;
  className?: string;
  bgColor?: string;
  borderRadius?: number;
  children: React.ReactNode;
  boxRef?: React.Ref<HTMLDivElement>;
}

const defaultBgColor = tailwindBackgroundColors["second-gray-bg"];
const defaultBorderRadius = 10;

const BoxStyledComponent: React.FC<IProps> = React.forwardRef(
  (
    {
      minWidth,
      height,
      title,
      className = "",
      bgColor = defaultBgColor,
      borderRadius = defaultBorderRadius,
      children,
      boxRef
    },
    ref
  ) => {
    return (
      <>
        <ComponentTopBarMenuLayout title={title} />
        <div
          style={{
            width: "100%",
            backgroundColor: bgColor,
            borderRadius: borderRadius
          }}
          ref={boxRef}
          className={clsx(
            className,
            "relative border-[1px] h-full border-border-gray flex-grow"
          )}
        >
          {children}
        </div>
      </>
    );
  }
);

export default BoxStyledComponent;
