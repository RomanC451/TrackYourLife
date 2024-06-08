import React from "react";
import { LogOutSvg } from "~/assets/sidebar";
import SideBarButton from "~/components/sidebar/components/SideBarButton";
import { useLogOutMutation } from "../../../features/authentication/hooks/useLogOutMutation";

const LogOutButton: React.FC = (): JSX.Element => {
  const { logOutMutation } = useLogOutMutation();

  return (
    <SideBarButton
      svg={<LogOutSvg />}
      text="Log out"
      onClick={() => {
        logOutMutation.mutate();
      }}
    ></SideBarButton>
  );
};

export default LogOutButton;
