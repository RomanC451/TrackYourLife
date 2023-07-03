import React from "react";
import { renderHook } from "@testing-library/react";

import { useObjectOfRefs } from "~/hooks";

import refsObjectToFetchBody from "../refsObjectToFetchBody";

describe("refsArrayToFetchBody tests", () => {
  test("Calling with an empty object should return an empty object", () => {
    expect(refsObjectToFetchBody({})).toEqual({});
  });

  test("Calling with an empty object should return an empty object", () => {
    const { result } = renderHook(() =>
      useObjectOfRefs({ email: "envkt@example.com", password: "password" })
    );
    const [refsObject, _] = result.current;

    expect(refsObject).toEqual({
      email: { current: "envkt@example.com" },
      password: { current: "password" }
    });

    expect(refsObjectToFetchBody(refsObject)).toEqual({
      email: "envkt@example.com",
      password: "password"
    });
  });
});
