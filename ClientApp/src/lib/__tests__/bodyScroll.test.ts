import { afterEach, beforeEach, describe, expect, it } from "vitest";

import {
  disableBodyScroll,
  enableBodyScroll,
  resetBodyScroll,
} from "../bodyScroll";

describe("bodyScroll", () => {
  beforeEach(() => {
    resetBodyScroll();
    document.body.style.overflow = "auto";
    document.body.style.paddingRight = "0px";
  });

  afterEach(() => {
    resetBodyScroll();
    document.body.style.overflow = "";
    document.body.style.paddingRight = "";
  });

  it("locks body scroll on first disable", () => {
    disableBodyScroll();

    expect(document.body.style.overflow).toBe("hidden");
  });

  it("keeps scroll locked until all locks are released", () => {
    disableBodyScroll();
    disableBodyScroll();

    enableBodyScroll();
    expect(document.body.style.overflow).toBe("hidden");

    enableBodyScroll();
    expect(document.body.style.overflow).toBe("auto");
  });

  it("restores original overflow and padding", () => {
    document.body.style.overflow = "scroll";
    document.body.style.paddingRight = "12px";

    disableBodyScroll();
    enableBodyScroll();

    expect(document.body.style.overflow).toBe("scroll");
    expect(document.body.style.paddingRight).toBe("12px");
  });

  it("resetBodyScroll clears styles and lock count", () => {
    disableBodyScroll();
    disableBodyScroll();

    resetBodyScroll();

    expect(document.body.style.overflow).toBe("");
    expect(document.body.style.paddingRight).toBe("");

    disableBodyScroll();
    expect(document.body.style.overflow).toBe("hidden");
  });
});
