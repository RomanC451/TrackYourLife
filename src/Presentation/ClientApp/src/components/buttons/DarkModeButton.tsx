import React, { useState } from "react";

const DarkModeButton: React.FC = (): JSX.Element => {
  const [checked, setChecked] = useState(true);

  return (
    <div className=" flex items-center overflow-hidden">
      <input
        type="checkbox"
        id="dark-mode-button"
        className="toggle toggle-sm ml-[2px]"
        checked={checked}
        onChange={() => {
          setChecked(!checked);
        }}
      />
      <span className="bold ml-[20px] font block flex-shrink-0 text-center font-['Nunito_Sans'] text-[14px]  font-[400] leading-[15.4px] w-auto">
        Dark Mode
      </span>
    </div>
  );
};

export default DarkModeButton;
