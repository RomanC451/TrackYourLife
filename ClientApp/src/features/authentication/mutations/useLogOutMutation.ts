import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import globalAxios from "axios";
import { AuthApi } from "../../../services/openapi/api";

const authApi = new AuthApi();

export const useLogOutMutation = () => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  const logOutMutation = useMutation({
    mutationFn: () => authApi.logOutUser(),
    onSuccess: () => {
      globalAxios.defaults.headers.common["Authorization"] = undefined;

      // TODO REMOVE: just for debug purpose
      localStorage.removeItem("jwtToken");

      queryClient.resetQueries({ queryKey: ["userData"] });
      navigate({ to: "/auth", replace: true });
    },
  });

  return {
    logOutMutation,
  };
};
