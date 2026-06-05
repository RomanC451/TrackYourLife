import { Link } from "@tanstack/react-router";
import { Calendar, Clock, Eye } from "lucide-react";

import { Card, CardContent } from "@/components/ui/card";
import { useYoutubePlayerHost } from "@/features/youtube/contexts/YoutubePlayerHostContext";
import { cn } from "@/lib/utils";

import type { WatchedVideoHistoryEntry } from "../watchHistoryTypes";

function formatViewCount(count: number): string {
  if (count >= 1_000_000) return `${(count / 1_000_000).toFixed(1)}M`;
  if (count >= 1000) return `${(count / 1000).toFixed(1)}K`;
  return count.toString();
}

function formatRelativeDate(dateString: string): string {
  const date = new Date(dateString);
  const now = new Date();
  const diffDays = Math.floor(
    (now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24),
  );
  if (diffDays === 0) return "Today";
  if (diffDays === 1) return "Yesterday";
  if (diffDays < 7) return `${diffDays} days ago`;
  if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
  if (diffDays < 365) return `${Math.floor(diffDays / 30)} months ago`;
  return `${Math.floor(diffDays / 365)} years ago`;
}

interface WatchHistoryEntryCardProps {
  entry: WatchedVideoHistoryEntry;
}

function WatchHistoryEntryCard({ entry }: WatchHistoryEntryCardProps) {
  const { openYoutubePlayer } = useYoutubePlayerHost();
  const video = entry.video;

  const handlePlayClick = () => {
    openYoutubePlayer({ videoId: entry.videoId });
  };

  if (!video) {
    return (
      <Card className="overflow-hidden">
        <div className="flex flex-col gap-3 p-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="min-w-0">
            <p className="font-mono text-sm text-muted-foreground">{entry.videoId}</p>
            <p className="text-sm text-destructive">Could not load video details</p>
            <p className="mt-1 flex items-center gap-1 text-xs text-muted-foreground">
              <Clock className="h-3 w-3" />
              Watched {formatRelativeDate(entry.watchedAtUtc)}
            </p>
          </div>
          <button
            type="button"
            className="text-sm font-medium text-primary hover:underline"
            onClick={handlePlayClick}
          >
            Open video
          </button>
        </div>
      </Card>
    );
  }

  return (
    <Card className="overflow-hidden transition-all hover:shadow-lg">
      <button
        type="button"
        className="relative block aspect-video w-full cursor-pointer overflow-hidden transition-transform hover:scale-[1.01]"
        onClick={handlePlayClick}
      >
        <img
          src={video.thumbnailUrl}
          alt={video.title}
          className="h-full w-full object-cover brightness-90"
        />
        <div className="absolute bottom-2 right-2 rounded bg-black/80 px-1.5 py-0.5 text-xs font-medium text-white">
          {video.duration}
        </div>
        <div className="absolute inset-0 flex items-center justify-center bg-black/20">
          <div className="rounded-full bg-primary/90 px-3 py-1 text-xs font-medium text-primary-foreground">
            Watched
          </div>
        </div>
      </button>
      <CardContent className="p-3">
        <button
          type="button"
          className="line-clamp-2 w-full cursor-pointer text-left text-sm font-semibold leading-tight text-muted-foreground hover:underline"
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
          <p className="mt-1 text-xs text-muted-foreground">{video.channelName}</p>
        )}
        <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
          <span className="flex items-center gap-1">
            <Eye className="h-3 w-3" />
            {formatViewCount(video.viewCount)}
          </span>
          <span className="flex items-center gap-1">
            <Calendar className="h-3 w-3" />
            {formatRelativeDate(video.publishedAt)}
          </span>
          <span className={cn("flex items-center gap-1 font-medium text-foreground")}>
            <Clock className="h-3 w-3" />
            Watched {formatRelativeDate(entry.watchedAtUtc)}
          </span>
        </div>
      </CardContent>
    </Card>
  );
}

export default WatchHistoryEntryCard;
