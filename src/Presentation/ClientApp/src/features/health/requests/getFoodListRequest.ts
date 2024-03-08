import { WretchError } from "wretch/resolver";
import { foodEndpoints, getErrorObject } from "~/data/apiSettings";
import { TFetchRequest } from "~/hooks/useApiRequests";

type getFoodListRequestProps = {
  fetchRequest: TFetchRequest<undefined, TFoodListResponse>;
  controllerRef?: React.MutableRefObject<AbortController | undefined>;
  setSearchError: React.Dispatch<React.SetStateAction<string>>;
  searchParam: string;
  page: number;
};

export type ServingSize = {
  nutritionMultiplier: number;
  unit: string;
  value: number;
  id: string;
  index: number;
};

export type FoodElement = {
  id: string;
  type: string;
  brandName: string;
  countryCode: string;
  name: string;
  nutritionalContents: {
    calcium: number;
    carbohydrates: number;
    cholesterol: number;
    fat: number;
    fiber: number;
    iron: number;
    monounsaturatedFat: number;
    netCarbs: number;
    polyunsaturatedFat: number;
    potassium: number;
    protein: number;
    saturatedFat: number;
    sodium: number;
    sugar: number;
    transFat: number;
    vitaminA: number;
    vitaminC: number;
    energy: {
      unit: string;
      value: number;
    };
  };
  servingSizes: ServingSize[];
};

type PagedFoodList = {
  items: FoodElement[];
  page: number;
  pageSize: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};

export type TFoodListResponse = { foodList: PagedFoodList };

const errorMessages: { [key: string]: string } = {
  "Food.NotFound": "Food not found.",
  default: "Server error."
};

const getFoodListRequest = async ({
  fetchRequest,
  controllerRef,
  setSearchError,
  searchParam,
  page
}: getFoodListRequestProps) => {
  const searchParams = new URLSearchParams({
    searchParam,
    page: page.toString(),
    pageSize: "10"
  });

  const endpoint = foodEndpoints.getFoodList + "?" + searchParams.toString();

  return fetchRequest({
    endpoint: endpoint,
    requestType: "GET",
    authorized: true,
    abortControllerRef: controllerRef,
    catchers: {
      notFound: (error: WretchError) => {
        const errorObject = getErrorObject(error);
        const errorMessage = errorMessages[errorObject.type];
        if (errorMessage !== undefined) setSearchError(errorMessage);
        else setSearchError(errorMessages["default"]);

        throw error;
      },
      badRequest: (error: WretchError) => {
        setSearchError(errorMessages["default"]);
        throw error;
      }
    }
  }).then((json: TFoodListResponse) => json);
};

export default getFoodListRequest;
