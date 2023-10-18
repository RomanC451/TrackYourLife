import React from "react";
import { Link } from "react-router-dom";
import { sidebarSectionsEnum } from "~/components/sidebar/sideBarSections";

interface ISideBarElementProps {
  svg: React.ReactElement;
  section: sidebarSectionsEnum;
  active: boolean;
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const SideBarElement: React.FunctionComponent<ISideBarElementProps> = ({
  svg,
  section,
  active,
  onClick
}) => {
  const filledSvg = active
    ? React.cloneElement(svg, {
        fill: "#6C48D9"
      })
    : React.cloneElement(svg, {
        fill: "#D9D9D9"
      });

  return (
    <button
      className={` mb-[11px] flex flex-shrink-0 h-[38px] w-full cursor-pointer items-center overflow-hidden ${
        active ? "" : "hover:translate-x-2 duration-500 "
      }`}
      onClick={onClick}
    >
      {active ? (
        <div className=" absolute left-0 h-[38px] w-full bg-violet bg-opacity-[0.26]">
          <div className="absolute right-0 h-full w-[5px] bg-violet" />
        </div>
      ) : null}
      <div className="flex h-[35px] w-[35px] items-center justify-center flex-shrink-0">
        {filledSvg}
      </div>
      <p
        className={`ml-[18px] bold font flex-shrink-0 font-['Nunito_Sans'] text-[14px]  font-[400] leading-[15.4px] ${
          active ? "text-sidebar-active-element" : "text-sidebar-gray"
        }`}
      >
        {section}
      </p>
    </button>
  );
};

export default SideBarElement;
