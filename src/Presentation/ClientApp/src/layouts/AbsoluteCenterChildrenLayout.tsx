import React from "react";
import { cn } from "../utils/utils";

interface Props {
  className?: string;
  children: React.ReactNode;
}

const AbsoluteCenterChildrenLayout: React.FC<Props> = ({
  className,
  children,
}): JSX.Element => {
  return (
    <div className={cn(className, "absolute w-full flex justify-center")}>
      {children}
    </div>
  );
};

export default AbsoluteCenterChildrenLayout;
