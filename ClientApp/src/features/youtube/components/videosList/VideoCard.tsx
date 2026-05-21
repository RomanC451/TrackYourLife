import { Link, useLocation } from "@tanstack/react-router";
import { Calendar, Eye } from "lucide-react";

import { Card, CardContent } from "@/components/ui/card";
import { useYoutubePlayerHost } from "@/features/youtube/contexts/YoutubePlayerHostContext";
import { cn } from "@/lib/utils";
import { YoutubeVideoPreview } from "@/services/openapi";

import AddToPlaylistDropdown from "../library/AddToPlaylistDropdown";
import usePlayVideoMutation from "../../mutations/usePlayVideoMutation";
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
}

function VideoCard({ video }: VideoCardProps) {
  const location = useLocation();
  const { openYoutubePlayer } = useYoutubePlayerHost();
  const playVideoMutation = usePlayVideoMutation();

  const isYoutubePage = location.pathname.includes("/youtube/");

  const handlePlayClick = () => {
    if (isYoutubeCardClickSuppressed()) {
      return;
    }

    if (isYoutubePage) {
      openYoutubePlayer({
        videoId: video.videoId,
      });
      return;
    }

    playVideoMutation.mutate(video.videoId);
  };

  return (
    <Card
      className={cn("overflow-hidden transition-all hover:shadow-lg", {
        "opacity-75": video.isWatched,
      })}
    >
      <button
        type="button"
        className="relative block aspect-video w-full cursor-pointer overflow-hidden transition-transform hover:scale-[1.02]"
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
      <CardContent className="p-3">
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
