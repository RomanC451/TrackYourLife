import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/router-devtools";
import {
  FetchRequestProps,
  TFetchRequestData,
  TFetchRequestResponse,
} from "~/hooks/useApiRequests";
import MissingPage from "~/pages/MissingPage";
import { wait } from "~/utils/wait";

export type AuthContext = {
  userLoggedIn: () => Promise<boolean>;
  fetchRequest: <
    TData extends TFetchRequestData,
    TResponse extends TFetchRequestResponse,
  >({
    endpoint,
    requestType,
    data,
    authorized,
    jsonResponse,
    abortControllerRef,
    abortSignal,
  }: FetchRequestProps<TData>) => Promise<Awaited<TResponse>>;
};

export const Route = createRootRouteWithContext<AuthContext>()({
  component: () => (
    <>
      <Outlet />
      <TanStackRouterDevtools />
    </>
  ),
  notFoundComponent: () => (
    <>
      <MissingPage />
      <TanStackRouterDevtools />
    </>
  ),
  loader: () => wait(1),
});
