import React, { useEffect } from "react";
import { Route, useNavigate } from "react-router-dom";
import useUserData from "~/auth/useUserData";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { userDataInitValue } from "~/contexts/AuthenticationContextProvider";

type IProtectedRoute = {
  children: React.ReactNode;
};

const ProtectedPage: React.FC<IProtectedRoute> = ({
  children
}): JSX.Element => {
  const navigate = useNavigate();

  const { refetchUserData, userLogedIn, userData } = useUserData();

  const { refreshTokenIsValid } = useApiContext();

  useEffect(() => {
    if (!refreshTokenIsValid()) {
      navigate("/auth");
      return;
    }
    refetchUserData();
  }, []);

  if (!userLogedIn() || userData.id === userDataInitValue.id) {
    return <></>;
  } else {
    return <>{children}</>;
  }
};

export default ProtectedPage;
