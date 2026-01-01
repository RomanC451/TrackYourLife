import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { AlertCircle, Calendar, Eye, ThumbsUp, User, X } from "lucide-react";

import HandleQuery from "@/components/handle-query";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import Spinner from "@/components/ui/spinner";
import { disableBodyScroll, enableBodyScroll } from "@/lib/bodyScroll";
import { YoutubeVideoDetails } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";

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
}

function VideoPlayerDialog({ videoId, onClose }: VideoPlayerDialogProps) {
  const navigate = useNavigate();

  const query = useQuery({
    ...youtubeQueryOptions.videoDetails(videoId),
  });

  useEffect(() => {
    disableBodyScroll();
    return () => enableBodyScroll();
  }, []);

  const handleClose = () => {
    if (onClose) {
      onClose();
    } else {
      navigate({ to: "/youtube/videos" });
    }
  };

  return HandleQuery({
    query: query,
    success: (videoDetails: YoutubeVideoDetails) => (
      <VideoDetails videoDetails={videoDetails} handleClose={handleClose} />
    ),
    error: (error: ApiError) => (
      <DefaultError handleClose={handleClose} error={error} />
    ),
    pending: () => <Spinner />,
  });
}

function VideoDetails({
  videoDetails,
  handleClose,
}: {
  videoDetails: YoutubeVideoDetails;
  handleClose: () => void;
}) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-0 md:p-4">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/80"
        onClick={handleClose}
        aria-hidden="true"
      />

      {/* Content */}
      <div className="relative z-10 flex h-[100dvh] w-full max-w-[1400px] flex-col overflow-hidden bg-background md:h-[95dvh] md:rounded-lg md:border md:shadow-lg lg:flex-row">
        {/* Close button */}
        <Button
          variant="ghost"
          size="icon"
          className="absolute right-2 top-2 z-20 rounded-full bg-black/50 text-white hover:bg-black/70"
          onClick={handleClose}
        >
          <X className="h-5 w-5" />
          <span className="sr-only">Close</span>
        </Button>

        {/* Video Player */}
        <div className="relative flex-1 bg-black">
          <iframe
            title={videoDetails.title}
            src={videoDetails.embedUrl.replace("youtube", "youtube-nocookie")}
            className="absolute inset-0 h-full w-full"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
          />
        </div>

        {/* Video Info Sidebar */}
        <div className="flex h-[40vh] w-full flex-col border-t bg-background p-4 lg:h-full lg:w-[350px] lg:border-l lg:border-t-0">
          <h2 className="text-lg font-semibold leading-tight">
            {videoDetails.title}
          </h2>

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
      </div>
    </div>
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
      <div className="relative z-10 flex h-[100dvh] w-full max-w-[600px] flex-col overflow-hidden bg-background p-6 md:h-auto md:rounded-lg md:border md:shadow-lg">
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
