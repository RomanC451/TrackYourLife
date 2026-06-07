import React from "react";
import { describe, expect, it } from "vitest";

import { changeSvgColor } from "../changeSvg";

describe("changeSvgColor", () => {
  it("clones an svg element with the requested fill color", () => {
    const svg = <svg data-testid="icon" />;

    const result = changeSvgColor(svg, "#ff0000");

    expect(React.isValidElement(result)).toBe(true);
    expect((result as React.ReactElement<{ fill: string }>).props.fill).toBe(
      "#ff0000",
    );
  });
});
