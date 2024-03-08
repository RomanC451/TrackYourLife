import { foodDiaryEndpoints } from "~/data/apiSettings";
import { TFetchRequest } from "~/hooks/useApiRequests";

const deleteFoodDiaryRequest = (
  fetchRequest: TFetchRequest<undefined, undefined>,
  foodDiaryId: string
) => {
  const endpoint = `${foodDiaryEndpoints.default}/${foodDiaryId}`;

  return fetchRequest({
    endpoint,
    requestType: "DELETE",
    authorized: true,
    jsonResponse: false
  });
};

export default deleteFoodDiaryRequest;
