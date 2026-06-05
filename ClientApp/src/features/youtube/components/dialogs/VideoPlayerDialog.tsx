import { useEffect, useRef, useState, type ReactNode } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { AlertCircle, Calendar, Eye, ThumbsUp, User, X } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import Spinner from "@/components/ui/spinner";
import { disableBodyScroll, enableBodyScroll } from "@/lib/bodyScroll";
import { YoutubeVideoDetails } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import SubscribeChannelDropdown from "../channels/SubscribeChannelDropdown";
import AddToPlaylistDropdown from "../library/AddToPlaylistDropdown";
import usePlayVideoMutation from "../../playback/usePlayVideoMutation";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";

function formatViewCount(count: number): string {
  if (count >= 1000000) {
    return `${(count / 1000000).toFixed(1)}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K`;
  }
  return count.toString();
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

interface VideoPlayerDialogProps {
  videoId: string;
  onClose?: () => void;
  onPlayFailed?: () => void;
  isVisible?: boolean;
}

function VideoPlayerShell({
  handleClose,
  children,
}: {
  handleClose: () => void;
  children: ReactNode;
}) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-0 md:p-4">
      <div
        className="absolute inset-0 bg-black/80"
        onClick={handleClose}
        aria-hidden="true"
      />
      <div className="relative z-10 flex h-dvh w-full max-w-[1400px] flex-col overflow-hidden bg-background md:h-[95dvh] md:rounded-lg md:border md:shadow-lg lg:flex-row">
        <Button
          variant="ghost"
          size="icon"
          className="absolute right-2 top-2 z-20 rounded-full bg-black/50 text-white hover:bg-black/70"
          onClick={handleClose}
        >
          <X className="h-5 w-5" />
          <span className="sr-only">Close</span>
        </Button>
        {children}
      </div>
    </div>
  );
}

function VideoPlayerLoading({ handleClose }: { handleClose: () => void }) {
  return (
    <VideoPlayerShell handleClose={handleClose}>
      <div className="flex flex-1 items-center justify-center bg-black p-2 md:p-4">
        <Spinner className="h-10 w-10 fill-violet-800" />
      </div>
      <div className="flex h-[40vh] w-full flex-col border-t bg-background p-4 lg:h-full lg:w-[350px] lg:border-l lg:border-t-0">
        <div className="h-6 w-3/4 animate-pulse rounded bg-muted" />
        <div className="mt-3 h-4 w-1/3 animate-pulse rounded bg-muted" />
        <div className="mt-4 flex gap-4">
          <div className="h-4 w-20 animate-pulse rounded bg-muted" />
          <div className="h-4 w-20 animate-pulse rounded bg-muted" />
        </div>
        <div className="mt-6 min-h-0 flex-1 space-y-2">
          <div className="h-4 w-full animate-pulse rounded bg-muted" />
          <div className="h-4 w-full animate-pulse rounded bg-muted" />
          <div className="h-4 w-2/3 animate-pulse rounded bg-muted" />
        </div>
      </div>
    </VideoPlayerShell>
  );
}

function VideoPlayerDialog({
  videoId,
  onClose,
  onPlayFailed,
  isVisible = true,
}: VideoPlayerDialogProps) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const playVideoMutation = usePlayVideoMutation();

  useEffect(() => {
    if (isVisible) {
      disableBodyScroll();
    } else {
      enableBodyScroll();
    }

    return () => enableBodyScroll();
  }, [isVisible]);

  useEffect(() => {
    playVideoMutation.mutate(videoId, {
      onError: () => {
        onPlayFailed?.();
      },
    });
    // Play once per videoId; dedupePlayVideoRequest handles Strict Mode double-invoke.
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional
  }, [videoId]);

  const handleClose = () => {
    if (onClose) {
      onClose();
    } else {
      navigate({ to: "/youtube/videos" });
    }
  };

  const videoDetails = playVideoMutation.isPending
    ? undefined
    : queryClient.getQueryData<YoutubeVideoDetails>(
      youtubeQueryKeys.videoDetails(videoId),
    );

  if (playVideoMutation.isPending) {
    return <VideoPlayerLoading handleClose={handleClose} />;
  }

  if (videoDetails) {
    return <VideoDetails videoDetails={videoDetails} handleClose={handleClose} />;
  }

  if (playVideoMutation.isError && playVideoMutation.error) {
    return (
      <DefaultError handleClose={handleClose} error={playVideoMutation.error} />
    );
  }

  return <VideoPlayerLoading handleClose={handleClose} />;
}

function VideoDetails({
  videoDetails,
  handleClose,
}: {
  videoDetails: YoutubeVideoDetails;
  handleClose: () => void;
}) {
  const playerUrl = videoDetails.embedUrl;
  const playerLoadCountRef = useRef(0);
  const [isPlayerReady, setIsPlayerReady] = useState(false);

  useEffect(() => {
    playerLoadCountRef.current = 0;
    setIsPlayerReady(false);

    // Fallback in case the iframe only emits one load event.
    const timeoutId = globalThis.setTimeout(() => {
      setIsPlayerReady(true);
    }, 1800);

    return () => {
      globalThis.clearTimeout(timeoutId);
    };
  }, [playerUrl]);

  const handlePlayerLoad = () => {
    playerLoadCountRef.current += 1;

    // Piped embed often triggers a second load after SPA route hydration.
    if (playerLoadCountRef.current >= 2) {
      setIsPlayerReady(true);
    }
  };

  return (
    <VideoPlayerShell handleClose={handleClose}>
      <div className="relative flex flex-1 items-center justify-center bg-black p-2 md:p-4">
        {!isPlayerReady && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-black">
            <Spinner className="h-8 w-8 fill-violet-800" />
          </div>
        )}
        <iframe
          title={videoDetails.title}
          src={playerUrl}
          className={`aspect-video w-full max-h-full max-w-full transition-opacity duration-200 ${isPlayerReady ? "opacity-100" : "opacity-0"}`}
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen
          onLoad={handlePlayerLoad}
        />
      </div>

      <div className="flex h-[40vh] w-full flex-col border-t bg-background p-4 @container/video-details lg:h-full lg:w-[350px] lg:border-l lg:border-t-0">
        <div className="flex flex-col gap-2 @[30rem]/video-details:flex-row @[30rem]/video-details:items-start">
          <h2 className="min-w-0 text-lg font-semibold leading-tight @[30rem]/video-details:flex-1">
            {videoDetails.title}
          </h2>
          <div className="flex w-full shrink-0 items-center justify-end gap-1.5 @[30rem]/video-details:w-auto @[30rem]/video-details:justify-start">
            <SubscribeChannelDropdown
              channelId={videoDetails.channelId}
              channelName={videoDetails.channelName}
              menuContentClassName="z-[100]"
              showLabel
            />
            <AddToPlaylistDropdown
              videoId={videoDetails.videoId}
              menuContentClassName="z-[100]"
              showLabel
            />
          </div>
        </div>

        <div className="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
          <User className="h-4 w-4" />
          <span>{videoDetails.channelName}</span>
        </div>

        <div className="mt-4 flex flex-wrap gap-4 text-sm text-muted-foreground">
          <div className="flex items-center gap-1">
            <Eye className="h-4 w-4" />
            <span>{formatViewCount(videoDetails.viewCount)} views</span>
          </div>
          <div className="flex items-center gap-1">
            <ThumbsUp className="h-4 w-4" />
            <span>{formatViewCount(videoDetails.likeCount)} likes</span>
          </div>
          <div className="flex items-center gap-1">
            <Calendar className="h-4 w-4" />
            <span>{formatDate(videoDetails.publishedAt)}</span>
          </div>
        </div>

        <div className="mt-4 min-h-0 flex-1">
          <h3 className="mb-2 text-sm font-medium">Description</h3>
          <ScrollArea className="h-[calc(100%-2rem)]">
            <p className="whitespace-pre-wrap pr-4 text-sm text-muted-foreground">
              {videoDetails.description || "No description available"}
            </p>
          </ScrollArea>
        </div>
      </div>
    </VideoPlayerShell>
  );
}

function DefaultError({
  handleClose,
  error,
}: {
  handleClose: () => void;
  error: ApiError;
}) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-0 md:p-4">
      <div
        className="absolute inset-0 bg-black/80"
        onClick={handleClose}
        aria-hidden="true"
      />
      <div className="relative z-10 flex h-dvh w-full max-w-[600px] flex-col overflow-hidden bg-background p-6 md:h-auto md:rounded-lg md:border md:shadow-lg">
        <Button
          variant="ghost"
          size="icon"
          className="absolute right-2 top-2 rounded-full"
          onClick={handleClose}
        >
          <X className="h-5 w-5" />
          <span className="sr-only">Close</span>
        </Button>

        <Alert variant="destructive" className="mt-8">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Error Loading Video</AlertTitle>
          <AlertDescription className="mt-2">
            {error?.response?.data?.detail ||
              "An error occurred while loading the video. Please try again."}
          </AlertDescription>
        </Alert>

        <div className="mt-6 flex justify-end">
          <Button onClick={handleClose}>Close</Button>
        </div>
      </div>
    </div>
  );
}

export default VideoPlayerDialog;
