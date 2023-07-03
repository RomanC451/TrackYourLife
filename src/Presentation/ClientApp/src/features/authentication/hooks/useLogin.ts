import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import { formStatesEnum } from "~/data/forms";
import { useObjectRef } from "~/hooks";
import { postFetch } from "~/services/postFetch";

export type userDataRefsType = {
  email: string;
  password: string;
};

interface useLoginInterface {
  formState: formStatesEnum;
  setFormState: React.Dispatch<React.SetStateAction<formStatesEnum>>;
  changeUserData: (
    name: keyof userDataRefsType,
    value: userDataRefsType[keyof userDataRefsType]
  ) => void;
  handleLogInRequest: () => void;
}

/**
 * Custom hook for handling the login form.
 * Stores the user data and provides the handle for the login request.
 *
 * @returns An object with properties:
 *   - formState: The current state of the form (enum).
 *   - setFormState: A function to set the state of the form (enum).
 *   - changeUserData: A function to handle changes in user data inputs.
 *   - handleLogInRequest: A function to handle the login request.
 */
const useLogin = (): useLoginInterface => {
  const navigate = useNavigate();

  const { defaultApi, setJwtToken } = useApiContext();

  const [formState, setFormState] = useState<formStatesEnum>(
    formStatesEnum.unknown
  );

  const [userDataRefs, changeUserData] = useObjectRef({
    email: "",
    password: ""
  });

  function handleLogInRequest() {
    postFetch(
      defaultApi,
      userDataRefs.current,
      userEndpoints.login,
      setJwtToken
    )
      .badRequest((error) => {
        try {
          var errorJson = JSON.parse(error.message);
          errorJson.type === "User.InvalidCredentials"
            ? setFormState(formStatesEnum.bad)
            : setFormState(formStatesEnum.somethingWrong);
        } catch (e) {}
      })
      .json((data) => {
        setFormState(formStatesEnum.good);
        setJwtToken(data.jwtToken);
        navigate("/home");
      });
  }

  return { formState, setFormState, changeUserData, handleLogInRequest };
};

export default useLogin;
