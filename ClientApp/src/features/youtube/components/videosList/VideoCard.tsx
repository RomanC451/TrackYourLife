import { Link } from "@tanstack/react-router";
import { Calendar, Eye } from "lucide-react";

import { Card, CardContent } from "@/components/ui/card";
import { useYoutubePlayback } from "@/features/youtube/playback/useYoutubePlayback";
import { cn } from "@/lib/utils";
import { YoutubeVideoPreview } from "@/services/openapi";

import AddToPlaylistDropdown from "../library/AddToPlaylistDropdown";
import { isYoutubeCardClickSuppressed } from "../../youtubeClickGuard";

function formatViewCount(count: number): string {
  if (count >= 1000000) {
    return `${(count / 1000000).toFixed(1)}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K`;
  }
  return count.toString();
}

function formatPublishedDate(dateString: string): string {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

  if (diffDays === 0) return "Today";
  if (diffDays === 1) return "Yesterday";
  if (diffDays < 7) return `${diffDays} days ago`;
  if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
  if (diffDays < 365) return `${Math.floor(diffDays / 30)} months ago`;
  return `${Math.floor(diffDays / 365)} years ago`;
}

interface VideoCardProps {
  video: YoutubeVideoPreview;
  layout?: "default" | "featured";
}

function VideoCard({ video, layout = "default" }: VideoCardProps) {
  const isFeatured = layout === "featured";
  const { openYoutubeVideo } = useYoutubePlayback();

  const handlePlayClick = () => {
    if (isYoutubeCardClickSuppressed()) {
      return;
    }

    openYoutubeVideo(video.videoId);
  };

  return (
    <Card
      className={cn("overflow-hidden transition-all hover:shadow-lg", {
        "opacity-75": video.isWatched,
        "xl:flex xl:flex-row": isFeatured,
      })}
    >
      <button
        type="button"
        className={cn(
          "relative block aspect-video w-full cursor-pointer overflow-hidden transition-transform hover:scale-[1.02]",
          isFeatured &&
            "xl:aspect-auto xl:min-h-[200px] xl:w-[min(48%,420px)] xl:shrink-0 xl:self-stretch",
        )}
        onClick={handlePlayClick}
      >
        <img
          src={video.thumbnailUrl}
          alt={video.title}
          className={cn("h-full w-full object-cover", {
            "brightness-75": video.isWatched,
          })}
        />
        <div className="absolute bottom-2 right-2 rounded bg-black/80 px-1.5 py-0.5 text-xs font-medium text-white">
          {video.duration}
        </div>
        {video.isWatched && (
          <div className="absolute inset-0 flex items-center justify-center bg-black/20">
            <div className="rounded-full bg-primary/90 px-3 py-1 text-xs font-medium text-primary-foreground">
              Watched
            </div>
          </div>
        )}
      </button>
      <CardContent
        className={cn("p-3", isFeatured && "xl:flex xl:flex-1 xl:flex-col xl:justify-center xl:p-5")}
      >
        <button
          type="button"
          className={cn(
            "line-clamp-2 w-full cursor-pointer text-left text-sm font-semibold leading-tight hover:underline",
            { "text-muted-foreground": video.isWatched },
          )}
          onClick={handlePlayClick}
        >
          {video.title}
        </button>
        {video.channelId ? (
          <Link
            to="/youtube/channels/$youtubeChannelId"
            params={{ youtubeChannelId: video.channelId }}
            className="mt-1 block truncate text-xs text-muted-foreground hover:text-foreground hover:underline"
            onClick={(e) => e.stopPropagation()}
          >
            {video.channelName}
          </Link>
        ) : (
          <p className="mt-1 text-xs text-muted-foreground">
            {video.channelName}
          </p>
        )}
        <div className="mt-2 flex items-center justify-between gap-2">
          <div className="flex items-center gap-3 text-xs text-muted-foreground">
            <div className="flex items-center gap-1">
              <Eye className="h-3 w-3" />
              <span>{formatViewCount(video.viewCount)}</span>
            </div>
            <div className="flex items-center gap-1">
              <Calendar className="h-3 w-3" />
              <span>{formatPublishedDate(video.publishedAt)}</span>
            </div>
          </div>
          <AddToPlaylistDropdown videoId={video.videoId} />
        </div>
      </CardContent>
    </Card>
  );
}

export default VideoCard;
