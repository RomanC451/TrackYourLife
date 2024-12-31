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

export type ApiErrorData = {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors: ApiError[];
};

export type ApiError = AxiosError<ApiErrorData>;

//  TODO just for debug purpose
export const toastDefaultServerError = (error?: Error) => {
  toast.error(`Server error: ${error}`, { className: "bg-red-500 text-white" });
};
