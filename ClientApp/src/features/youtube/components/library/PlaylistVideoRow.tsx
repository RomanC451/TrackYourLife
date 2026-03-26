import { useQuery } from "@tanstack/react-query";
import { Calendar, Eye, Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import type { YoutubePlaylistVideoItemDto, YoutubeVideoPreview } from "@/services/openapi";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";

function formatViewCount(count: number): string {
  if (count >= 1_000_000) return `${(count / 1_000_000).toFixed(1)}M`;
  if (count >= 1000) return `${(count / 1000).toFixed(1)}K`;
  return count.toString();
}

function formatPublishedDate(dateString: string): string {
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

interface PlaylistVideoRowProps {
  item: YoutubePlaylistVideoItemDto;
  /** When set (e.g. from list endpoint), skips per-row video-details fetch. */
  preview?: YoutubeVideoPreview | null;
  onWatch: (videoId: string) => void;
  onRemove: (videoId: string) => void;
  playVideoPending: boolean;
  removeVideoPending: boolean;
}

function PlaylistVideoRow({
  item,
  preview,
  onWatch,
  onRemove,
  playVideoPending,
  removeVideoPending,
}: PlaylistVideoRowProps) {
  const { data: details, isPending, isError } = useQuery({
    ...youtubeQueryOptions.videoDetails(item.youtubeId),
    enabled: preview == null,
  });

  const display = preview ?? details;

  if (preview == null && isPending) {
    return (
      <Card className="overflow-hidden">
        <div className="flex gap-4 p-3">
          <div className="aspect-video w-40 shrink-0 animate-pulse rounded-md bg-muted" />
          <div className="min-w-0 flex-1 space-y-2 py-1">
            <div className="h-4 w-3/4 animate-pulse rounded bg-muted" />
            <div className="h-3 w-1/2 animate-pulse rounded bg-muted" />
          </div>
        </div>
      </Card>
    );
  }

  if (preview == null && (isError || !details)) {

    return (
      <Card className="overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-3 p-4">
          <div className="min-w-0">
            <p className="font-mono text-sm text-muted-foreground">{item.youtubeId}</p>
            <p className="text-sm text-destructive">Could not load video details</p>
          </div>
          <div className="flex shrink-0 gap-2">
            <Button
              type="button"
              size="sm"
              variant="secondary"
              onClick={() => onWatch(item.youtubeId)}
              disabled={playVideoPending}
            >
              Watch
            </Button>
            <Button
              type="button"
              size="sm"
              variant="ghost"
              className="text-destructive hover:text-destructive"
              onClick={() => onRemove(item.youtubeId)}
              disabled={removeVideoPending}
            >
              Remove
            </Button>
          </div>
        </div>
      </Card>
    );
  }

  if (display == null) {
    return null;
  }


  return (
    <Card className="overflow-hidden transition-all hover:shadow-md">
      <div className="flex flex-col gap-3 p-3 sm:flex-row sm:items-stretch">
        <button
          type="button"
          className="relative aspect-video w-full shrink-0 overflow-hidden rounded-md sm:w-44"
          onClick={() => onWatch(item.youtubeId)}
          disabled={playVideoPending}
        >
          <img
            src={display.thumbnailUrl}
            alt=""
            className="h-full w-full object-cover"
          />
          <div className="absolute bottom-2 right-2 rounded bg-black/80 px-1.5 py-0.5 text-xs font-medium text-white">
            {display.duration}
          </div>
        </button>

        <div className="flex min-w-0 flex-1 flex-col justify-between gap-3">
          <div>
            <button
              type="button"
              className={cn(
                "text-left text-sm font-semibold leading-snug hover:underline",
                "line-clamp-2",
              )}
              onClick={() => onWatch(item.youtubeId)}
              disabled={playVideoPending}
            >
              {display.title}
            </button>
            <p className="mt-1 text-xs text-muted-foreground">{display.channelName}</p>
            <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
              <span className="flex items-center gap-1">
                <Eye className="h-3 w-3" />
                {formatViewCount(display.viewCount)}
              </span>
              <span className="flex items-center gap-1">
                <Calendar className="h-3 w-3" />
                {formatPublishedDate(display.publishedAt)}
              </span>
            </div>
          </div>

          <div className="flex flex-wrap gap-2">
            <Button
              type="button"
              size="sm"
              variant="secondary"
              onClick={() => onWatch(item.youtubeId)}
              disabled={playVideoPending}
            >
              {playVideoPending ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                "Watch"
              )}
            </Button>
            <Button
              type="button"
              size="sm"
              variant="ghost"
              className="text-destructive hover:text-destructive"
              onClick={() => onRemove(item.youtubeId)}
              disabled={removeVideoPending}
            >
              Remove from playlist
            </Button>
          </div>
        </div>
      </div>
    </Card>
  );
}

export default PlaylistVideoRow;
