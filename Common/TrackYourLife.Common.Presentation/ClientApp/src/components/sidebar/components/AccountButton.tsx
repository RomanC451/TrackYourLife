import React from "react";
import { AccountSvg } from "~/assets/sidebar";
import SideBarButton from "~/components/sidebar/components/SideBarButton";
import { useAuthenticationContext } from "~/contexts/AuthenticationContextProvider";

const AccountButton: React.FC = (): JSX.Element => {
  const { userData } = useAuthenticationContext();
  return (
    <SideBarButton
      svg={<AccountSvg />}
      text={
        <div className="flex w-full flex-col items-start">
          <p className="w-full break-words text-left ">
            {userData?.firstName} {userData?.lastName}
          </p>
          <p className="w-full break-words text-left text-[13px] font-bold">
            {userData?.email}
          </p>
        </div>
      }
      onClick={() => {}}
    ></SideBarButton>
  );
};

export default AccountButton;
