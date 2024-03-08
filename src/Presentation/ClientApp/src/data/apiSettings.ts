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
};

export type TErrorObject = {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors: TErrorObject[];
};

export const getErrorObject = (error: Error): TErrorObject | undefined => {
  try {
    const errorObject: TErrorObject = JSON.parse(error.message);
    return errorObject;
  } catch (e) {
    return undefined;
  }
};

export const toastDefaultServerError = () => {
  toast.error(`Server error`, { className: "bg-red-500 text-white" });
};
