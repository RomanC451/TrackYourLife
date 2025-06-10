import { AxiosError } from "axios";
import { toast } from "sonner";

export type ApiErrorData = {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors: {name: string, message: string}[];
};

export type ApiError = AxiosError<ApiErrorData>;

//  TODO just for debug purpose
export const toastDefaultServerError = (error?: Error) => {
  toast.error(`Server error: ${error}`, { className: "bg-red-500 text-white" });
};
