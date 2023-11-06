import React from "react";
import { LogOutSvg } from "~/assets/sidebar";
import useUserData from "~/auth/useUserData";
import SideBarButton from "~/components/sidebar/components/SideBarButton";

const LogOutButton: React.FC = (): JSX.Element => {
  const { logOutUser } = useUserData();

  return (
    <SideBarButton
      svg={<LogOutSvg />}
      text="Log out"
      onClick={() => {
        logOutUser();
      }}
    ></SideBarButton>
  );
};

export default LogOutButton;
