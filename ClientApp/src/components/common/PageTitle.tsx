import { cn } from "@/lib/utils";

import { CardTitle } from "../ui/card";

function PageTitle({
  title,
  children,
  className,
}: {
  title: string;
  children?: React.ReactNode;
  className?: string;
}) {
  return (
    <div className="flex w-full flex-wrap items-center justify-between gap-4">
      <CardTitle className={cn("text-3xl font-bold", className)}>
        {title}
      </CardTitle>
      <div className="ml-auto">{children}</div>
    </div>
  );
}

export default PageTitle;
