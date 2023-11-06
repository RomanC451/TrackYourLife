import { useNavigate } from "react-router-dom";
import { useApiContext } from "~/contexts/ApiContextProvider";
import {
  useAuthenticationContext,
  userDataInitValue
} from "~/contexts/AuthenticationContextProvider";
import { getFetch } from "~/services/getFetch";

import { userEndpoints } from "../data/apiSettings";

const useUserData = () => {
  const { userData, setUserData } = useAuthenticationContext();
  const { authorizedApi, setJwtToken } = useApiContext();

  const navigate = useNavigate();

  const updateUserData = (newData: Partial<UserData>) => {
    setUserData((data) => {
      return { ...data, ...newData };
    });
  };

  const setUserId = (id: string) => {
    setUserData((data) => {
      return { ...data, id: id };
    });
  };

  const userLogedIn = (): boolean => {
    if (localStorage.getItem("refreshToken") !== "") return true;
    else return false;
  };

  const logOutUser = () => {
    setUserData(userDataInitValue);
    setJwtToken("");
    navigate("/auth");
  };

  const refetchUserData = () => {
    getFetch(authorizedApi, userEndpoints.getById + userData.id, navigate)
      .badRequest((error) => {
        //!!TODO: handle the bad request
      })
      .json(
        (response: {
          id: string;
          email: string;
          firstName: string;
          lastName: string;
        }) => {
          updateUserData({
            id: response.id,
            email: response.email,
            firstName: response.firstName,
            lastName: response.lastName
          });
        }
      )
      .catch(() => {
        //TODO: handle error
      });
  };

  return { setUserId, userData, refetchUserData, userLogedIn, logOutUser };
};

export default useUserData;
