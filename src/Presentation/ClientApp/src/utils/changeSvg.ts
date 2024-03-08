import React from "react";

export const changeSvgColor = (
  svg: React.FunctionComponentElement<any> | React.ReactElement,
  color: string
) => {
  return React.cloneElement(svg, {
    fill: color
  });
};
