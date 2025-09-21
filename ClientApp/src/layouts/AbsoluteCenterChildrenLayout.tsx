import React from "react";

import { cn } from "@/lib/utils";

interface Props {
  className?: string;
  children: React.ReactNode;
}

const AbsoluteCenterChildrenLayout: React.FC<Props> = ({
  className,
  children,
}): React.JSX.Element => {
  return (
    <div className={cn(className, "absolute flex w-full justify-center")}>
      {children}
    </div>
  );
};

export default AbsoluteCenterChildrenLayout;
