import React from "react";
import { Card } from "~/chadcn/ui/card";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";
import { cn } from "~/utils/utils";

interface IProps {
  title: string;
  className?: string;
  children: React.ReactNode;
  onClick?: () => void;
}

const BoxStyledComponent: React.FC<IProps> = ({
  title,
  className = "",
  onClick,
  children,
}) => {
  return (
    <>
      <ComponentTopBarMenuLayout title={title} />
      <Card
        className={cn(
          className,
          "border-border-gray relative h-[calc(100%-33px)] w-full flex-grow border-[1px]",
        )}
        onClick={onClick}
      >
        {children}
      </Card>
    </>
  );
};

export default BoxStyledComponent;
