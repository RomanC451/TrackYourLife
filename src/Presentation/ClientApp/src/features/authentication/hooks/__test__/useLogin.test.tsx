import {
  ApiContextProvider,
  useApiContext
} from "~/contexts/ApiContextProvider";
import { formStatesEnum } from "~/data/forms";
import { postFetch } from "~/services/postFetch";

import { act, renderHook } from "@testing-library/react";

import useLogin from "../useLogin";

jest.mock("~/contexts/ApiContextProvider", () => ({
  useApiContext: jest.fn(() => ({ defaultApi: {}, setJwtToken: () => {} })),
  ApiContextProvider: ({ children }: { children: React.ReactNode }) => (
    <>{children}</>
  )
}));

jest.mock(
  "~/services/postFetch",
  () => ({
    postFetch: jest
      .fn()
      .mockImplementation((_, __, ___, ____) => {})
      .mockReturnValue({
        badRequest: jest
          .fn((cb) =>
            cb({ message: JSON.stringify({ type: "User.InvalidCredentials" }) })
          )
          .mockReturnValue({
            json: jest.fn((cb) => cb({ username: "testUser" }))
          })
      })
  })

  // unauthorized: jest.fn()
);

describe("useLogin", () => {
  it("should handle login request", async () => {
    const { result } = renderHook(() => useLogin(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <ApiContextProvider>{children}</ApiContextProvider>
      )
    });

    // Change the user data
    act(() => {
      result.current.changeUserData("email", "test@example.com");
      result.current.changeUserData("password", "password");
    });

    // Call the `handleLogInRequest` function
    act(() => {
      debugger;
      result.current.handleLogInRequest();
    });

    // Wait for the next update

    // // Assert that `postFetch` was called with the correct arguments
    expect(postFetch).toHaveBeenCalledWith(
      {}, // defaultApi
      { email: "test@example.com", password: "password" }, // userData
      "users/login", // endpoint
      expect.any(Function) // setJwtToken
    );

    // // Assert that the form state was updated correctly
    expect(result.current.formState).toBe(formStatesEnum.good);
  });
});
