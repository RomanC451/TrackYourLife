import { Suspense } from "react";
import { createFileRoute, Link } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import ChannelPageHeader from "@/features/youtube/components/channel/ChannelPageHeader";
import ChannelVideosList from "@/features/youtube/components/channel/ChannelVideosList";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";

const CHANNEL_VIDEOS_MAX_RESULTS = 25;

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/channels_/$youtubeChannelId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { youtubeChannelId } = Route.useParams();

  return (
    <PageCard>
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <ChannelDetail youtubeChannelId={youtubeChannelId} />
      </Suspense>
    </PageCard>
  );
}

function ChannelDetail({ youtubeChannelId }: { youtubeChannelId: string }) {
  const { data: channels } = useSuspenseQuery(
    youtubeQueryOptions.channels("all"),
  );
  const { data: videos } = useSuspenseQuery(
    youtubeQueryOptions.channelVideos(youtubeChannelId, CHANNEL_VIDEOS_MAX_RESULTS),
  );

  const subscribedChannel = channels.find(
    (c) => c.youtubeChannelId === youtubeChannelId,
  );

  const channelName =
    subscribedChannel?.name ?? videos[0]?.channelName ?? "Channel";
  const thumbnailUrl = subscribedChannel?.thumbnailUrl;

  return (
    <>
      <PageTitle
        title={
          <Breadcrumb className="mb-4">
            <BreadcrumbList>
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to="/youtube/channels">Channels</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem className="min-w-0 max-w-full">
                <BreadcrumbPage className="truncate">{channelName}</BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        }
      />

      <ChannelPageHeader
        youtubeChannelId={youtubeChannelId}
        channelName={channelName}
        thumbnailUrl={thumbnailUrl}
        subscribedChannel={subscribedChannel}
      />

      <div className="mt-6">
        <ChannelVideosList videos={videos} />
      </div>
    </>
  );
}
