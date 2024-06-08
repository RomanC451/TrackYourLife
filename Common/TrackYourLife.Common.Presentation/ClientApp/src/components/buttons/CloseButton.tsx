import React from "react";

import { XIcon } from "lucide-react";
import { Button } from ".";

interface CloseButtonInterface {
  onClick: React.MouseEventHandler;
  className?: string;
}

const CloseButton: React.FC<CloseButtonInterface> = ({
  onClick,
  className,
}) => {
  return (
    <Button
      Icon={<XIcon />}
      color="rgb(153, 171, 180)"
      bgHoverColor="light-gray"
      size="2xl"
      borderRadius="50%"
      onClick={onClick}
      className={className}
    />
  );
};

export default CloseButton;
