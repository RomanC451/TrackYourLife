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
    <div className="flex flex-wrap items-center justify-between gap-4">
      <CardTitle className={cn("text-3xl font-bold", className)}>
        {title}
      </CardTitle>
      {children && (
        <div className="ml-auto flex justify-between gap-4 @lg/page-card:w-auto">
          {children}
        </div>
      )}
    </div>
  );
}

export default PageTitle;
