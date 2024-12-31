import React from "react";
import { cn } from "~/utils";

interface IProps {
  fontStyle?: string;
  title: string;
}

const defautlFontStyle = "font-[Nunito_Sans] font-semibold";

const ComponentTopBarMenuLayout: React.FC<IProps> = ({
  fontStyle = "",
  title,
}): JSX.Element => {
  fontStyle = fontStyle ? fontStyle : defautlFontStyle;

  return (
    <div className=" inline-flex flex-col items-center w-full">
      <div
        className={cn(fontStyle, "flex w-full justify-between items-center ")}
      >
        <p className="font-[24px]">{title}</p>
        <button className="flex items-center">
          <p className="text-gray font-[20px]">Daily</p>
          {/* <DorpDownArrowSvg className="pt-[2px]" /> */}
        </button>
      </div>
    </div>
  );
};

export default ComponentTopBarMenuLayout;
