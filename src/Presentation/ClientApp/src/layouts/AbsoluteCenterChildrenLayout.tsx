import clsx from "clsx";
import React from "react";

interface Props {
  className?: string;
  children: React.ReactNode;
}

const AbsoluteCenterChildrenLayout: React.FC<Props> = ({
  className,
  children
}): JSX.Element => {
  return (
    <div className={clsx(className, "absolute w-full flex justify-center")}>
      {children}
    </div>
  );
};

export default AbsoluteCenterChildrenLayout;
