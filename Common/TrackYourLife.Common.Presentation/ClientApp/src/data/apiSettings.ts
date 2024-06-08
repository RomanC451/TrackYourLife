import { AxiosError } from "axios";
import { toast } from "sonner";

export const apiUrl = import.meta.env.VITE_API_URL;

export const userEndpoints = {
  register: "/user/register",
  login: "/user/login",
  logout: "/user/logout",
  remove: "/user/remove",
  update: "/user/update",
  refreshToken: "/user/refresh-token",
  verifyEmail: "/user/verify-email",
  default: "/user",
  resendVerificationEmail: "/user/resend-verification-email",
};

export const foodEndpoints = {
  getFoodList: "/food/search",
  default: "/food",
};

export const foodDiaryEndpoints = {
  default: "/foodDiary",
  byDate: "/foodDiary/by-date",
  totalCalories: "/foodDiary/total-calories",
};

export const userGoalEndpoints = {
  default: "/userGoal",
  latest: "/userGoal/latest",
};

export type ApiError = AxiosError<{
  type: string;
  title: string;
  status: number;
  detail: string;
  errors: ApiError[];
}>;

export const getErrorObject = (error: AxiosError): ApiError | undefined => {
  console.log(error.response?.data);
  if (!error.response?.data) return;
  try {
    const errorObject: ApiError = JSON.parse(error.message);
    return errorObject;
  } catch (e) {
    return;
  }
};

export const toastDefaultServerError = () => {
  toast.error(`Server error`, { className: "bg-red-500 text-white" });
};
