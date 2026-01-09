import { useState } from "react";

import Spinner from "./ui/spinner";
import VideoPlayer from "./video-player";

function VideoPlayerWithLoading({ url }: { url: string }) {
  const [loading, setLoading] = useState(true);
  return (
    <div className="relative inline-block h-full w-full">
      {loading && (
        <div className="absolute left-0 top-0 flex h-full w-full items-center justify-center rounded-md border bg-secondary">
          <Spinner className="h-10 w-10 fill-violet-800" />
        </div>
      )}
      <VideoPlayer
        url={url}
        onLoaded={() => setLoading(false)}
        onError={() => setLoading(false)}
      />
    </div>
  );
}

export default VideoPlayerWithLoading;
