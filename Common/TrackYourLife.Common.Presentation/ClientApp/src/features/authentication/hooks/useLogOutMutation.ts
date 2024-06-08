import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import globalAxios from "axios";
import { AuthenticationApi } from "../../../services/openapi/api";

const authApi = new AuthenticationApi();

export const useLogOutMutation = () => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  const logOutMutation = useMutation({
    mutationFn: () => authApi.logOut(),
    onSuccess: () => {
      globalAxios.defaults.headers.common["Authorization"] = undefined;

      //!! TO BE REMOVED: just for debug purpose
      localStorage.removeItem("jwtToken");

      queryClient.resetQueries({ queryKey: ["userData"] });
      navigate({ to: "/auth", replace: true });
    },
  });

  return {
    logOutMutation,
  };
};
