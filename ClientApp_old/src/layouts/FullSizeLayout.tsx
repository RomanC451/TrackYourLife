import { cn } from "~/utils";

type FullSizeLayoutProps = React.HTMLAttributes<"div">;

const FullSizeLayout: React.FC<FullSizeLayoutProps> = ({
  children,
  className,
}) => {
  return (
    <div className={cn("flex flex-grow overflow-hidden", className)}>
      {children}
    </div>
  );
};

export default FullSizeLayout;
