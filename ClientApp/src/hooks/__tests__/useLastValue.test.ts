import { renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import useLastValue from "../useLastValue";

describe("useLastValue", () => {
  it("returns undefined on the first render", () => {
    const { result } = renderHook(() => useLastValue("first"));

    expect(result.current).toBeUndefined();
  });

  it("returns the previous value after an update", () => {
    const { result, rerender } = renderHook(({ value }) => useLastValue(value), {
      initialProps: { value: "first" },
    });

    rerender({ value: "second" });
    expect(result.current).toBe("first");

    rerender({ value: "third" });
    expect(result.current).toBe("second");
  });
});
