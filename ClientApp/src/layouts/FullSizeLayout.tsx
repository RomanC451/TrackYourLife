import { cn } from "@/lib/utils";

function FullSizeLayout({ children, className }: React.HTMLAttributes<"div">) {
  return (
    // ?? dow we need "flex-grow" here?
    <div className={cn("flex w-full flex-grow", className)}>{children}</div>
  );
}

export default FullSizeLayout;
