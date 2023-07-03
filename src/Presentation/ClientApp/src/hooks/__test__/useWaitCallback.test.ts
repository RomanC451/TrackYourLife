import { renderHook } from "@testing-library/react";
import useWaitCallback from "../useWaitCallback";

describe("useWaitCallback tests", () => {
  const { result } = renderHook(useWaitCallback);
  const [canCall, reset] = result.current;

  test("canCall should method returns true at first call", () => {
    expect(canCall()).toBe(true);
  });

  test("canCall should method returns false at second call", () => {
    expect(canCall()).toBe(false);
  });

  test("canCall should method returns true if the hook was reset", () => {
    reset();
    expect(canCall()).toBe(true);
  });
});
