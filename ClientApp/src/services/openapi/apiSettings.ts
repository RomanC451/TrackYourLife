import { AxiosError, isAxiosError } from "axios";
import { toast } from "sonner";

export type ApiErrorData = {
  type: string;
  title: string;
  status: number;
  detail: string;
  errors: { name: string; message: string }[];
};

export type ApiError = AxiosError<ApiErrorData>;

function is4xxStatus(status: number | undefined): boolean {
  return status !== undefined && status >= 400 && status < 500;
}

/** Dev default mutation error toast: prefer API problem `detail` for 4xx. */
export const toastDefaultServerError = (error?: unknown) => {
  if (
    import.meta.env.MODE === "development" &&
    isAxiosError(error) &&
    is4xxStatus(error.response?.status)
  ) {
    const detail = error.response?.data?.detail;
    if (typeof detail === "string" && detail.trim().length > 0) {
      toast.error(`Server error: ${error.status} - ${detail}`, {
        className: "bg-red-500 text-white",
      });
      return;
    }
  }

  let message = "Something went wrong";
  if (error !== undefined) {
    if (error instanceof Error) {
      message = `Server error: ${error.message}`;
    } else if (typeof error === "string") {
      message = `Server error: ${error}`;
    } else {
      message = `Server error: ${JSON.stringify(error)}`;
    }
  }
  toast.error(message, { className: "bg-red-500 text-white" });
};
