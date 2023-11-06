import React from "react";

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
  onClick
}): JSX.Element => {
  const filledSvg = active
    ? React.cloneElement(svg, {
        fill: "#6C48D9"
      })
    : React.cloneElement(svg, {
        fill: "#D9D9D9"
      });

  return (
    <button
      className={` mb-[11px] flex flex-shrink-0 h-min-[38px] w-full cursor-pointer items-center overflow-hidden ${
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
      <div
        className={`ml-[18px] bold font flex-shrink-0 font-['Nunito_Sans'] text-[14px]  font-[400] leading-[15.4px] w-[98px] text-left ${
          active ? "text-sidebar-active-element" : "text-sidebar-gray"
        }`}
      >
        {text}
      </div>
    </button>
  );
};

export default SideBarButton;
