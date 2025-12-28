import React from "react";

import OptimizedImage from "./optimized-image";

export function ImageWithSpinner(
  props: React.ImgHTMLAttributes<HTMLImageElement>,
  shouldLazyLoadImages?: boolean,
) {
  // Ensure src is defined
  if (!props.src) {
    return (
      <div className="flex h-full w-full items-center justify-center bg-gray-200">
        No image
      </div>
    );
  }

  return (
    <OptimizedImage
      src={props.src}
      alt={props.alt || ""}
      className={props.className}
      loading={shouldLazyLoadImages ? "lazy" : "eager"}
      placeholder="empty"
    />
  );
}
