import React from "react";

export const changeSvgColor = (
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  svg: React.FunctionComponentElement<any> | React.ReactElement,
  color: string,
) => {
  return React.cloneElement(svg, {
    fill: color,
  });
};
