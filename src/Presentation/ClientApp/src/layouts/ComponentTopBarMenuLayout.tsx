import clsx from "clsx";
import React from "react";
import { DorpDownArrowSvg } from "~/assets";

interface IProps {
  fontStyle?: string;
  children: React.ReactNode;
  title: string;
}

const defautlFontStyle = "font-[Nunito_Sans] font-semibold";

const ComponentTopBarMenuLayout: React.FC<IProps> = ({
  fontStyle = "",
  children,
  title
}): JSX.Element => {
  fontStyle = fontStyle ? fontStyle : defautlFontStyle;

  return (
    <div className=" inline-flex flex-col items-center">
      <div
        className={clsx(fontStyle, "flex w-full justify-between items-center ")}
      >
        <p className="text-white font-[24px]">{title}</p>
        <button className="flex items-center">
          <p className="text-gray font-[20px]">Daily</p>
          <DorpDownArrowSvg className="pt-[2px]" />
        </button>
      </div>
      {children}
    </div>
  );
};

export default ComponentTopBarMenuLayout;
