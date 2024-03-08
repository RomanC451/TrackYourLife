import { foodDiaryEndpoints } from "~/data/apiSettings";
import { TMealtTypes } from "~/features/health/data/enums";
import { requestTypes, TFetchRequest } from "~/hooks/useApiRequests";
import { DateOnly } from "~/utils/date";

export type TPostFoodDiaryData = {
  foodId: string;
  mealType: TMealtTypes;
  servingSizeId: string;
  quantity: number;
  date: DateOnly;
};

export type TPostFoodDiaryResponse = {
  foodDiaryEntryId: string;
};

const postFoodDiaryRequest = (
  fetchRequest: TFetchRequest<TPostFoodDiaryData, TPostFoodDiaryResponse>,
  data: TPostFoodDiaryData
) => {
  return fetchRequest({
    endpoint: foodDiaryEndpoints.default,
    requestType: requestTypes.POST,
    data: data,
    authorized: true,
    jsonResponse: true
  });
};

export default postFoodDiaryRequest;
