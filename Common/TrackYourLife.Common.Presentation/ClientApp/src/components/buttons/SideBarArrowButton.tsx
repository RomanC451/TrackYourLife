import { XIcon } from "lucide-react";
import React from "react";
import { Button } from "~/chadcn/ui/button";
import { colors } from "~/constants/tailwindColors";
import { cn } from "~/utils";

interface SideBarCloseButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  toggleSideBarButton: () => void;
}

const SideBarCloseButton: React.FC<SideBarCloseButtonProps> = ({
  toggleSideBarButton,
  className,
  ...buttonProps
}): JSX.Element => {
  return (
    <Button
      className={cn("", className)}
      onClick={toggleSideBarButton}
      {...buttonProps}
    >
      <XIcon fill={colors.violet} className="size-full" />
    </Button>
  );
};

export default SideBarCloseButton;
