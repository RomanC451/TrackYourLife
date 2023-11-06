import React from "react";

interface IProps {
  sideBarOpened: boolean;
}

const SideBarHeader: React.FC<IProps> = ({ sideBarOpened }): JSX.Element => {
  return (
    <p className="w-auto h-[66px] mt-[70px] block flex-shrink-0 text-center font-['Source_Serif_4'] text-[30px] font-[400] leading-[33px] text-white">
      {sideBarOpened ? "TRACK YOUR LIFE" : "T"}
    </p>
  );
};

export default SideBarHeader;
