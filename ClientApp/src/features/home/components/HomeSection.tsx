import type { ReactNode } from "react";

import { cn } from "@/lib/utils";

type HomeSectionProps = {
  title: string;
  description?: string;
  action?: ReactNode;
  className?: string;
  children: ReactNode;
};

function HomeSection({
  title,
  description,
  action,
  className,
  children,
}: HomeSectionProps) {
  return (
    <section className={cn("space-y-4", className)}>
      <div className="flex items-end justify-between gap-2">
        <div className="min-w-0 flex-1">
          <h2 className="text-lg font-semibold">{title}</h2>
          {description ? (
            <p className="text-sm text-muted-foreground">{description}</p>
          ) : null}
        </div>
        {action ? <div className="shrink-0">{action}</div> : null}
      </div>
      {children}
    </section>
  );
}

export default HomeSection;
