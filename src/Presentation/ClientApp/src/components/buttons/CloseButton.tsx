import React from "react";

import { MdOutlineCancel } from "react-icons/md";

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
      Icon={<MdOutlineCancel />}
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
