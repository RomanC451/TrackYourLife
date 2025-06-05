import React, { useState } from "react";

import Spinner from "./ui/spinner";

export function ImageWithSpinner(
  props: React.ImgHTMLAttributes<HTMLImageElement>,
) {
  const [loading, setLoading] = useState(true);

  return (
    <div className="relative inline-block h-full w-full">
      {loading && (
        <div className="absolute left-0 top-0 flex h-full w-full items-center justify-center border bg-secondary">
          <Spinner className="h-10 w-10 fill-violet-800" />
        </div>
      )}
      <img
        onLoad={() => setLoading(false)}
        onError={() => setLoading(false)}
        style={loading ? { visibility: "hidden" } : {}}
        {...props}
      />
    </div>
  );
}
