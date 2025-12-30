import { useSuspenseQuery } from "@tanstack/react-query";

import { VideoCategory } from "@/services/openapi";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import VideoCard from "./VideoCard";

interface VideosListProps {
  category?: VideoCategory | null;
  maxResultsPerChannel?: number;
}

function VideosList({
  category,
  maxResultsPerChannel = 5,
}: VideosListProps) {
  const { data: videos } = useSuspenseQuery(
    youtubeQueryOptions.videos(maxResultsPerChannel, category)
  );

  if (videos.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">No videos found</p>
        <p className="mt-2 text-sm text-muted-foreground">
          Add some channels to see their latest videos here
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-4 @lg/page-card:grid-cols-2 @3xl/page-card:grid-cols-3 @5xl/page-card:grid-cols-4">
      {videos.map((video) => (
        <VideoCard key={video.videoId} video={video} />
      ))}
    </div>
  );
}

export default VideosList;

