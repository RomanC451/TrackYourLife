import { cn } from "@/lib/utils";

function FullSizeLayout({ children, className }: React.HTMLAttributes<"div">) {
  return (
    // ?? dow we need "grow" here?
    <div className={cn("flex w-full grow", className)}>{children}</div>
  );
}

export default FullSizeLayout;
