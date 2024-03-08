import { foodDiaryEndpoints } from "~/data/apiSettings";
import { TMealtTypes } from "~/features/health/data/enums";
import { requestTypes, TFetchRequest } from "~/hooks/useApiRequests";

export type TPutFoodDiaryData = {
  foodDiaryEntryId: string;
  quantity: number;
  servingSizeId: string;
  mealType: TMealtTypes;
};

const putFoodDiaryRequest = (
  fetchRequest: TFetchRequest<TPutFoodDiaryData, undefined>,
  data: TPutFoodDiaryData
) => {
  return fetchRequest({
    endpoint: foodDiaryEndpoints.default,
    requestType: requestTypes.PUT,
    data: data,
    authorized: true,
    jsonResponse: false
  });
};

export default putFoodDiaryRequest;
