import { PropsWithChildren } from "react";

import { cn } from "@/lib/utils";

import { Card, CardContent, CardTitle } from "../ui/card";

function DebugCard({
  title,
  children,
  className,
  ...props
}: PropsWithChildren<React.ComponentProps<"div"> & { title: string }>) {
  return (
    <Card
      className={cn("flex-grow space-x-4 space-y-4 p-4", className)}
      {...props}
    >
      <CardTitle>{title}</CardTitle>
      <CardContent className="flex gap-4">{children}</CardContent>
    </Card>
  );
}

export default DebugCard;
