import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import globalAxios from "axios";
import { useLocalStorage } from "usehooks-ts";
import { v4 as uuidv4 } from "uuid";

import { AuthApi } from "../../../services/openapi/api";

const authApi = new AuthApi();

export const useLogOutMutation = () => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  const [deviceId] = useLocalStorage("deviceId", uuidv4(), {
    deserializer: (v) => v,
    serializer: (v) => v,
  });

  const logOutMutation = useMutation({
    mutationFn: () => authApi.logOutUser({ deviceId, logOutAllDevices: true }),
    onSuccess: () => {
      globalAxios.defaults.headers.common["Authorization"] = undefined;

      queryClient.resetQueries({ queryKey: ["userData"] });
      navigate({ to: "/auth", replace: true });
    },
  });

  return {
    logOutMutation,
  };
};
