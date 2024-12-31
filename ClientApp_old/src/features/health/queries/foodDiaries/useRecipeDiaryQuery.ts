import { useQuery } from "@tanstack/react-query";
import { queryClient } from "~/queryClient";
import { RecipeDiariesApi } from "~/services/openapi";
import { QUERY_KEYS } from "../../data/queryKeys";

const recipeDiariesApi = new RecipeDiariesApi();

export default function useRecipeDiaryQuery(recipeDiaryId: string) {
  const recipeDiaryQuery = useQuery({
    queryKey: [QUERY_KEYS.recipeDiary, recipeDiaryId],
    queryFn: () =>
      recipeDiariesApi
        .getRecipeDiaryById(recipeDiaryId)
        .then((res) => res.data),
  });

  return recipeDiaryQuery;
}

export const invalidateRecipeDiaryQuery = (recipeDiaryId: string) => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.recipeDiary, recipeDiaryId],
  });
};
