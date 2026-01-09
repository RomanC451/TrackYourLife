import React from "react";

import OptimizedImage from "./optimized-image";

interface ImageWithSpinnerProps
  extends React.ImgHTMLAttributes<HTMLImageElement> {
  shouldLazyLoadImages?: boolean;
  objectFit?: "contain" | "cover" | "fill" | "none" | "scale-down";
}

export function ImageWithSpinner({
  shouldLazyLoadImages,
  objectFit,
  ...props
}: ImageWithSpinnerProps) {
  // Ensure src is defined
  if (!props.src) {
    return (
      <div className="flex h-full w-full items-center justify-center bg-gray-200">
        No image
      </div>
    );
  }

  // Extract objectFit from className if present (e.g., "object-contain" -> "contain")
  let extractedObjectFit = objectFit;
  if (!extractedObjectFit && props.className) {
    if (props.className.includes("object-contain")) {
      extractedObjectFit = "contain";
    } else if (props.className.includes("object-cover")) {
      extractedObjectFit = "cover";
    } else if (props.className.includes("object-fill")) {
      extractedObjectFit = "fill";
    } else if (props.className.includes("object-none")) {
      extractedObjectFit = "none";
    } else if (props.className.includes("object-scale-down")) {
      extractedObjectFit = "scale-down";
    }
  }

  return (
    <OptimizedImage
      src={props.src}
      alt={props.alt || ""}
      className={props.className}
      loading={shouldLazyLoadImages ? "lazy" : "eager"}
      placeholder="empty"
      objectFit={extractedObjectFit}
    />
  );
}
