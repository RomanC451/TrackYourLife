import { createContext, ReactNode, useContext, useState } from "react";
import { Assert } from "~/utils";

interface ContextInterface {
  userState: UserStateType;
  updateUserData: <K extends keyof UserStateType>(
    attribute: K,
    value: UserStateType[K]
  ) => void;
}

type UserStateType = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
};

const UserStateContext = createContext<ContextInterface>(
  {} as ContextInterface
);

const userInitialState = {
  id: "",
  email: "",
  firstName: "",
  lastName: ""
};

export const UserContextProvider = ({ children }: { children: ReactNode }) => {
  const [userState, setUserState] = useState(userInitialState);

  const updateUserData = <K extends keyof UserStateType>(
    attribute: K,
    value: UserStateType[K]
  ) => {
    setUserState({ ...userState, [attribute]: value });
  };

  return (
    <UserStateContext.Provider value={{ userState, updateUserData }}>
      {children}
    </UserStateContext.Provider>
  );
};

export const useNavbarStateContext = () => {
  const context = useContext(UserStateContext);
  Assert.isNotUndefined(
    context,
    "useCount must be used within a CountProvider!"
  );
  Assert.isNotEmptyObject(
    context,
    "useCount must be used within a CountProvider!"
  );
  return context;
};
