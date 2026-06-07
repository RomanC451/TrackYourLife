import { describe, expect, it } from "vitest";

import { setManuallySet } from "../dtoUtils";

describe("setManuallySet", () => {
  it("returns null and undefined unchanged", () => {
    expect(setManuallySet(null)).toBeNull();
    expect(setManuallySet(undefined)).toBeUndefined();
  });

  it("returns primitives unchanged", () => {
    expect(setManuallySet(42)).toBe(42);
    expect(setManuallySet("hello")).toBe("hello");
  });

  it("adds default isLoading and isDeleting to objects with id", () => {
    const result = setManuallySet({ id: "1", name: "Test" });

    expect(result).toEqual({
      id: "1",
      name: "Test",
      isLoading: false,
      isDeleting: false,
    });
  });

  it("preserves existing isLoading and isDeleting flags", () => {
    const result = setManuallySet({
      id: "1",
      isLoading: true,
      isDeleting: true,
    });

    expect(result).toEqual({
      id: "1",
      isLoading: true,
      isDeleting: true,
    });
  });

  it("recursively processes nested objects and arrays", () => {
    const result = setManuallySet({
      id: "parent",
      children: [{ id: "child-1" }, { id: "child-2", isLoading: true }],
      meta: { label: "group" },
    });

    expect(result).toEqual({
      id: "parent",
      isLoading: false,
      isDeleting: false,
      children: [
        { id: "child-1", isLoading: false, isDeleting: false },
        { id: "child-2", isLoading: true, isDeleting: false },
      ],
      meta: { label: "group" },
    });
  });
});
