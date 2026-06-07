import { render, screen } from "@testing-library/react";
import { UseQueryResult } from "@tanstack/react-query";
import { describe, expect, it } from "vitest";

import { ApiError } from "@/services/openapi/apiSettings";

import HandleMultipleQueries from "../handle-multiple-queries";

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

describe("HandleMultipleQueries", () => {
  it("renders success when all queries have data", () => {
    render(
      <HandleMultipleQueries
        queries={{
          users: createQuery({ data: ["alice"], isPending: false, isSuccess: true, status: "success" }),
          posts: createQuery({ data: ["post-1"], isPending: false, isSuccess: true, status: "success" }),
        }}
        success={(data) => (
          <div>
            Users: {(data.users as string[]).length}, Posts:{" "}
            {(data.posts as string[]).length}
          </div>
        )}
      />,
    );

    expect(screen.getByText("Users: 1, Posts: 1")).toBeInTheDocument();
  });

  it("renders loading when any query is pending", () => {
    render(
      <HandleMultipleQueries
        queries={{
          users: createQuery({ data: ["alice"], isPending: false, isSuccess: true, status: "success" }),
          posts: createQuery({ isPending: true, status: "pending" }),
        }}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Loading ...")).toBeInTheDocument();
  });

  it("renders a custom pending state", () => {
    render(
      <HandleMultipleQueries
        queries={{
          users: createQuery({ isPending: true, status: "pending" }),
        }}
        pending={() => <div>Waiting for queries</div>}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Waiting for queries")).toBeInTheDocument();
  });

  it("renders the default error state", () => {
    render(
      <HandleMultipleQueries
        queries={{
          users: createQuery({
            isError: true,
            isPending: false,
            status: "error",
            error: { message: "Users failed" } as ApiError,
          }),
          posts: createQuery({ data: ["post-1"], isPending: false, isSuccess: true, status: "success" }),
        }}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Error: users: Users failed")).toBeInTheDocument();
  });

  it("renders a custom error state", () => {
    render(
      <HandleMultipleQueries
        queries={{
          users: createQuery({
            isError: true,
            isPending: false,
            status: "error",
            error: { message: "Users failed" } as ApiError,
          }),
        }}
        error={(errors) => (
          <div>Custom: {errors.users.message}</div>
        )}
        success={() => <div>Success</div>}
      />,
    );

    expect(screen.getByText("Custom: Users failed")).toBeInTheDocument();
  });
});
