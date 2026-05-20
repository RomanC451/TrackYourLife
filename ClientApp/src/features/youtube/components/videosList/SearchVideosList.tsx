import { keepPreviousData } from "@tanstack/react-query";

import { useCustomQuery } from "@/hooks/useCustomQuery";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import VideoCard from "./VideoCard";

interface SearchVideosListProps {
  searchQuery: string;
  maxResults?: number;
  keepPreviousResults?: boolean;
}

function SearchVideosList({
  searchQuery,
  maxResults = 10,
  keepPreviousResults = false,
}: SearchVideosListProps) {
  const {
    query: { data: videos, isError },
  } = useCustomQuery({
    ...youtubeQueryOptions.videoSearch(searchQuery, maxResults),
    placeholderData: keepPreviousResults ? keepPreviousData : undefined,
  });

  if (isError) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">
          An error occurred while searching for videos
        </p>
      </div>
    );
  }

  if (videos == null) {
    return null;
  }

  if (videos.length === 0) {
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
