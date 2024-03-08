import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { useApiRequests } from "~/hooks/useApiRequests";
import { postLogOutRequest } from "../requests/postLogOutRequest";

export const useLogOutMutation = () => {
  const { fetchRequest } = useApiRequests();
  const { setJwtToken } = useApiContext();
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  const logOutMutation = useMutation({
    mutationFn: () => postLogOutRequest({ fetchRequest }),
    onSuccess: () => {
      setJwtToken("");
      queryClient.resetQueries({ queryKey: ["userData"] });
      navigate({ to: "/auth", replace: true });
    },
  });

  return {
    logOutMutation,
  };
};
