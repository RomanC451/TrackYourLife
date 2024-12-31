import React from "react";
import { SettingsSvg } from "~/assets/sidebar";
import SideBarButton from "~/components/sidebar/components/SideBarButton";

const SettingsButton: React.FC = (): JSX.Element => {
  return (
    <SideBarButton
      svg={<SettingsSvg />}
      text="Settings"
      onClick={() => {}}
    ></SideBarButton>
  );
};

export default SettingsButton;
