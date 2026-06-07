import { describe, expect, it } from "vitest";
import Assert from "../assert";

describe("Assert.reactChildrenLengthLowerThan", () => {
  it("does not throw when child count is below the limit", () => {
    expect(() =>
      Assert.reactChildrenLengthLowerThan(<span>only child</span>, 2),
    ).not.toThrow();
  });

  it("throws when child count reaches the limit", () => {
    expect(() =>
      Assert.reactChildrenLengthLowerThan(
        [<span key="one">one</span>, <span key="two">two</span>],
        2,
      ),
    ).toThrow("This layout component can only have one child.");
  });

  it("uses a custom error message", () => {
    expect(() =>
      Assert.reactChildrenLengthLowerThan(
        <>
          <span>one</span>
          <span>two</span>
        </>,
        1,
        "Too many children",
      ),
    ).toThrow("Too many children");
  });
});

describe("Assert.isNotUndefined", () => {
  it("does not throw for defined values", () => {
    expect(() => Assert.isNotUndefined("value")).not.toThrow();
  });

  it("throws for undefined", () => {
    expect(() => Assert.isNotUndefined(undefined)).toThrow(
      "Expected variable to be defined but got undefined",
    );
  });

  it("uses a custom error message", () => {
    expect(() => Assert.isNotUndefined(undefined, "Missing value")).toThrow(
      "Missing value",
    );
  });
});

describe("Assert.isNotEmptyObject", () => {
  it("does not throw for non-empty objects", () => {
    expect(() => Assert.isNotEmptyObject({ a: 1 })).not.toThrow();
  });

  it("throws for empty objects", () => {
    expect(() => Assert.isNotEmptyObject({})).toThrow(
      "The object must not be empty.",
    );
  });
});

describe("Assert.contextIsDefined", () => {
  it("does not throw for a non-empty context object", () => {
    expect(() => Assert.contextIsDefined({ user: "test" }, "Auth")).not.toThrow();
  });

  it("throws for an empty context object", () => {
    expect(() => Assert.contextIsDefined({}, "Auth")).toThrow(
      "Auth must be used within a AuthProvider",
    );
  });
});
