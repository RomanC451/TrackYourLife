import { PropsWithChildren } from "react";
import { cn } from "~/utils";

type FullSizeLayoutProps = PropsWithChildren & React.HTMLAttributes<"div">;

const FullSizeLayout: React.FC<FullSizeLayoutProps> = ({
  children,
  className,
}) => {
  return <div className={cn("size-full", className)}>{children}</div>;
};

export default FullSizeLayout;
