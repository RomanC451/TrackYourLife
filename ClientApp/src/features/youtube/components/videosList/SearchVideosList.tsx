import { Loader2 } from "lucide-react";

import { useCustomQuery } from "@/hooks/useCustomQuery";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import VideoCard from "./VideoCard";

interface SearchVideosListProps {
  searchQuery: string;
  maxResults?: number;
}

function SearchVideosList({
  searchQuery,
  maxResults = 10,
}: SearchVideosListProps) {
  const {
    query: { data: videos, isError },
    pendingState: { isPending: isLoading },
  } = useCustomQuery(youtubeQueryOptions.videoSearch(searchQuery, maxResults));

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">
          An error occurred while searching for videos
        </p>
      </div>
    );
  }

  if (!videos || videos.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">No videos found</p>
        <p className="mt-2 text-sm text-muted-foreground">
          Try a different search query
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

export default SearchVideosList;
