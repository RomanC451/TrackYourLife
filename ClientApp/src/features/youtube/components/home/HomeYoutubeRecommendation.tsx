import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { Loader2, Star } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import VideoCard from "@/features/youtube/components/videosList/VideoCard";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";

function HomeYoutubeRecommendation() {
  const recommendationQuery = useQuery(youtubeQueryOptions.homeRecommendation());

  if (recommendationQuery.isPending) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (recommendationQuery.isError) {
    return (
      <Alert variant="destructive">
        <AlertTitle>Could not load recommendation</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>Check your connection and try again.</p>
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="w-fit"
            onClick={() => recommendationQuery.refetch()}
          >
            Retry
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  if (recommendationQuery.data === null) {
    return (
      <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-12 text-center">
        <Star className="mb-3 h-10 w-10 text-muted-foreground" />
        <p className="text-lg font-medium">No recommendations yet</p>
        <p className="mt-2 max-w-md text-sm text-muted-foreground">
          Star your subscribed channels to get a video pick on your home page
          each time you visit.
        </p>
        <Button asChild className="mt-4" variant="outline">
          <Link to="/youtube/channels">Go to channels</Link>
        </Button>
      </div>
    );
  }

  return <VideoCard video={recommendationQuery.data} />;
}

export default HomeYoutubeRecommendation;
