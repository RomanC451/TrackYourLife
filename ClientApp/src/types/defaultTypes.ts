import { QueryKey } from "@tanstack/react-query";
import { ExternalToast, toast } from "sonner";

import { ApiError } from "@/services/openapi/apiSettings";

export type TJsonObject = Record<string, unknown>;

export type ObjectValues<T> = T[keyof T];

export type PartialWithRequired<T, K extends keyof T> = Partial<T> &
  Required<Pick<T, K>>;

declare module "@tanstack/react-query" {
  interface Register {
    defaultError: ApiError;
  }
}

type toastMessage = Parameters<(typeof toast)["message"]>[0];

type toastType = Exclude<
  NonNullable<
    Exclude<
      ReturnType<(typeof toast)["getHistory"]>[0],
      { dismiss: boolean }
    >["type"]
  >,
  "normal" | "action"
>;

declare module "@tanstack/react-query" {
  interface Register {
    mutationMeta: {
      onSuccessToast?: {
        message: toastMessage;
        data?: ExternalToast;
        type: toastType;
      };
      noDefaultErrorToast?: boolean;
      invalidateQueriesOnError?: boolean;
      onErrorToast?: {
        message: toastMessage;
        data?: ExternalToast;
        type: toastType;
      };
      invalidateQueries: Array<QueryKey> | null;
      awaitInvalidationQuery?: QueryKey;
    };
  }
}
