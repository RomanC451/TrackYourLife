import React from "react";
import { useTheme } from "~/chadcn/theme-provider";
import { changeSvgColor } from "~/utils/changeSvg";

interface ISideBarButtonProps {
  svg: React.ReactElement;
  text: string | JSX.Element;
  active?: boolean;
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const SideBarButton: React.FC<ISideBarButtonProps> = ({
  svg,
  text,
  active = false,
  onClick,
}): JSX.Element => {
  const { theme } = useTheme();

  const filledSvg = changeSvgColor(
    svg,
    active ? "#6C48D9" : theme === "dark" ? "#D9D9D9" : "#000000",
  );

  return (
    <button
      className={` h-min-[38px] mb-[11px] flex w-full flex-shrink-0 cursor-pointer items-center overflow-hidden ${
        active ? "" : "duration-500 hover:translate-x-2 "
      }`}
      onClick={onClick}
    >
      {active ? (
        <div className=" bg-violet absolute left-0 h-[38px] w-full bg-opacity-[0.26]">
          <div className="bg-violet absolute right-0 h-full w-[5px]" />
        </div>
      ) : null}
      <div className="flex h-[35px] w-[35px] flex-shrink-0 items-center justify-center">
        {filledSvg}
      </div>
      <div
        className={`bold font ml-[18px] w-[98px] flex-shrink-0 text-left  font-['Nunito_Sans'] text-[14px] font-[400] leading-[15.4px] ${
          active ? "text-sidebar-active-element" : "text-sidebar-gray"
        }`}
      >
        {text}
      </div>
    </button>
  );
};

export default SideBarButton;
