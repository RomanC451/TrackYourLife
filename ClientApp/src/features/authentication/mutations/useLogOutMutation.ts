import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import globalAxios from "axios";
import { useLocalStorage } from "usehooks-ts";
import { v4 as uuidv4 } from "uuid";

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";
import { useCustomMutation } from "@/hooks/useCustomMutation";

import { AuthApi } from "../../../services/openapi/api";
import { authModes } from "../data/enums";

const authApi = new AuthApi();

export const useLogOutMutation = () => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();
  const { setQueryEnabled } = useAuthenticationContext();

  const [deviceId] = useLocalStorage("deviceId", uuidv4(), {
    deserializer: (v) => v,
    serializer: (v) => v,
  });

  const logOutMutation = useCustomMutation({
    mutationFn: () => authApi.logOutUser({ deviceId, logOutAllDevices: true }),
    onSuccess: () => {
      globalAxios.defaults.headers.common["Authorization"] = undefined;

      setQueryEnabled(false);

      setTimeout(() => {
        queryClient.removeQueries({ queryKey: ["userData"] });
        navigate({
          to: "/auth",
          search: { authMode: authModes.logIn },
          replace: true,
        });
      }, 100);
    },
  });

  return logOutMutation;
};
