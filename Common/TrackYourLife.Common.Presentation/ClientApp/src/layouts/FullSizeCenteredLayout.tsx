import React, { PropsWithChildren } from "react";

import { cn } from "~/utils";

type FullSizeCenteredProps = PropsWithChildren & React.HTMLAttributes<"div">;

const FullSizeCenteredLayout: React.FC<FullSizeCenteredProps> = ({
  children,
  className,
}) => {
  return (
    <div
      className={cn("flex flex-grow items-center justify-center ", className)}
    >
      {children}
    </div>
  );
};

export default FullSizeCenteredLayout;
