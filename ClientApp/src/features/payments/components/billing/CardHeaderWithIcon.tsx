import type { ReactNode } from "react";

import { CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

type CardHeaderWithIconProps = {
  icon: ReactNode;
  title: string;
  description: string;
  right?: ReactNode;
};

export function CardHeaderWithIcon({
  icon,
  title,
  description,
  right,
}: CardHeaderWithIconProps) {
  return (
    <CardHeader className="flex flex-row items-start justify-between gap-3">
      <div className="flex items-start gap-3">
        <div className="rounded-lg bg-primary/10 p-2 text-primary">{icon}</div>
        <div className="space-y-1">
          <CardTitle className="text-base">{title}</CardTitle>
          <CardDescription>{description}</CardDescription>
        </div>
      </div>
      {right}
    </CardHeader>
  );
}
