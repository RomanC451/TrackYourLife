import React from "react";
import { useTheme } from "~/chadcn/theme-provider";
import { cn } from "~/utils";
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
    <div className="relative w-full">
      {active ? (
        <div className="absolute h-[35px] w-2 bg-violet-900"> </div>
      ) : null}
      <button
        className={cn(
          " h-min-[38px] flex w-full flex-shrink-0 cursor-pointer items-center  overflow-hidden pl-[20px]",
          active ? "bg-violet-900/50" : "duration-500 hover:translate-x-2 ",
        )}
        onClick={onClick}
      >
        <div className="flex h-[35px] w-[35px] flex-shrink-0 items-center justify-center">
          {filledSvg}
        </div>
        <div
          className={
            "bold font ml-[18px] w-[98px] flex-shrink-0 text-left  font-['Nunito_Sans'] text-[14px] font-[400] leading-[15.4px] "
          }
        >
          {text}
        </div>
      </button>
    </div>
  );
};

export default SideBarButton;
