import React from "react";
import { AccountSvg } from "~/assets/sidebar";
import useUserData from "~/auth/useUserData";
import SideBarButton from "~/components/sidebar/components/SideBarButton";

const AccountButton: React.FC = (): JSX.Element => {
  const { userData } = useUserData();
  return (
    <SideBarButton
      svg={<AccountSvg />}
      text={
        <div className="flex flex-col items-start w-full">
          <p className="break-words w-full text-left ">
            {userData.firstName} {userData.lastName}
          </p>
          <p className="text-[13px] break-words w-full text-left font-bold">
            {userData.email}
          </p>
        </div>
      }
      onClick={() => {}}
    ></SideBarButton>
  );
};

export default AccountButton;
