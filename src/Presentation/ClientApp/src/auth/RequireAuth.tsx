import { Navigate, Outlet, useLocation } from "react-router-dom";
import useUserData from "~/auth/useUserData";

const RequireAuth = () => {
  const { userData, userLogedIn } = useUserData();
  const location = useLocation();

  return userLogedIn() ? (
    <Outlet />
  ) : (
    <Navigate to="/auth" state={{ from: location }} replace />
  );
};

export default RequireAuth;
