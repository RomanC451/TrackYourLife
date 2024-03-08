import { foodDiaryEndpoints } from "~/data/apiSettings";
import { TFetchRequest } from "~/hooks/useApiRequests";
import { DateOnly } from "~/utils/date";

import { TMealtTypes } from "../data/enums";
import { FoodElement, ServingSize } from "./getFoodListRequest";

export type TFoodDiaryResponseEntry = {
  id: string;
  food: FoodElement;
  mealType: TMealtTypes;
  quantity: number;
  servingSize: ServingSize;
  date: DateOnly;
};

export type TGetFoodDiaryListResponse = {
  breakfast: TFoodDiaryResponseEntry[];
  lunch: TFoodDiaryResponseEntry[];
  dinner: TFoodDiaryResponseEntry[];
  snacks: TFoodDiaryResponseEntry[];
};

type getFoodDiaryListRequestProps = {
  fetchRequest: TFetchRequest<undefined, TGetFoodDiaryListResponse>;
  controllerRef?: React.MutableRefObject<AbortController | undefined>;
  date: DateOnly;
};

const getFoodDiaryListRequest = async ({
  fetchRequest,
  controllerRef,
  date
}: getFoodDiaryListRequestProps) => {
  const searchParams = new URLSearchParams({
    date: date
  });

  const endpoint = foodDiaryEndpoints.byDate + "?" + searchParams.toString();

  return fetchRequest({
    endpoint: endpoint,
    requestType: "GET",
    authorized: true,
    abortControllerRef: controllerRef
  }).then((json: TGetFoodDiaryListResponse) => json);
};

export default getFoodDiaryListRequest;
