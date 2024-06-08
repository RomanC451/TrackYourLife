import { useQuery } from "@tanstack/react-query";
import { UserGoalApi, UserGoalType } from "~/services/openapi";

const userGoalApi = new UserGoalApi();

const useUserCaloriesGoalQuery = () => {
  const userCaloriesGoalQuery = useQuery({
    queryKey: ["userCaloriesGoal"],
    queryFn: () =>
      userGoalApi.getActiveGoal(UserGoalType.Calories).then((res) => res.data),
    // getLatestCaloriesGoalRequest({
    //   fetchRequest,
    //   type: UserGoalTypes.calories,
    // }),
  });

  return { userCaloriesGoalQuery };
};

export default useUserCaloriesGoalQuery;
