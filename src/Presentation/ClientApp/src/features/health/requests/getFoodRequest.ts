import React from "react";
import { foodEndpoints } from "~/data/apiSettings";
import { TFetchRequest } from "~/hooks/useApiRequests";

import { FoodElement } from "./";

type getFoodRequestProps = {
  fetchRequest: TFetchRequest<undefined, TFoodResponse>;
  controllerRef?: React.MutableRefObject<AbortController | undefined>;
  foodId: string;
};

type TFoodResponse = { food: FoodElement };

const getFoodRequest = async ({
  fetchRequest,
  controllerRef,
  foodId,
}: getFoodRequestProps) => {
  const endpoint = foodEndpoints.default + "/" + foodId;

  const json = await fetchRequest({
    endpoint: endpoint,
    requestType: "GET",
    authorized: true,
    abortControllerRef: controllerRef,
  });
  return json;
};

export default getFoodRequest;
