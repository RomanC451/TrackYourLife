import { render, screen } from "@testing-library/react";
import { UseQueryResult } from "@tanstack/react-query";
import { describe, expect, it } from "vitest";

import { ApiError } from "@/services/openapi/apiSettings";

import HandleQuery from "../handle-query";

function createQuery<T>(
  overrides: Partial<UseQueryResult<T, ApiError>>,
): UseQueryResult<T, ApiError> {
  return {
    data: undefined,
    error: null,
    isError: false,
    isPending: false,
    isLoading: false,
    isSuccess: false,
    status: "pending",
    ...overrides,
  } as UseQueryResult<T, ApiError>;
}

describe("HandleQuery", () => {
  it("renders the success state", () => {
    render(
      <HandleQuery
        query={createQuery({ data: { title: "Hello" }, isPending: false, isSuccess: true, status: "success" })}
        success={(data) => <div>Success: {data.title}</div>}
      />,
    );

    expect(screen.getByText("Success: Hello")).toBeInTheDocument();
  });

  it("renders the default loading state", () => {
    render(
      <HandleQuery
        query={createQuery({ isPending: true, status: "pending" })}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Loading...")).toBeInTheDocument();
  });

  it("renders the empty state when delayed loading is false", () => {
    render(
      <HandleQuery
        query={createQuery({ isPending: true, status: "pending" })}
        isDelayedLoading={false}
        empty={() => <div>No content yet</div>}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("No content yet")).toBeInTheDocument();
  });

  it("renders a custom pending state", () => {
    render(
      <HandleQuery
        query={createQuery({ isPending: true, status: "pending" })}
        pending={() => <div>Custom loading</div>}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Custom loading")).toBeInTheDocument();
  });

  it("renders the default error state", () => {
    render(
      <HandleQuery
        query={createQuery({
          isError: true,
          isPending: false,
          status: "error",
          error: { message: "Boom" } as ApiError,
        })}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Error: Boom")).toBeInTheDocument();
  });

  it("renders a custom error state", () => {
    render(
      <HandleQuery
        query={createQuery({
          isError: true,
          isPending: false,
          status: "error",
          error: { message: "Boom" } as ApiError,
        })}
        error={(error) => <div>Custom error: {error.message}</div>}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Custom error: Boom")).toBeInTheDocument();
  });
});
