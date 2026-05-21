import { Badge } from "@/components/ui/badge";
import type { YoutubeChannelDto } from "@/services/openapi";

import ChannelHeaderActions from "./ChannelHeaderActions";

interface ChannelPageHeaderProps {
  youtubeChannelId: string;
  channelName: string;
  thumbnailUrl?: string;
  subscribedChannel: YoutubeChannelDto | undefined;
}

function ChannelPageHeader({
  youtubeChannelId,
  channelName,
  thumbnailUrl,
  subscribedChannel,
}: ChannelPageHeaderProps) {
  return (
    <div className="flex flex-col gap-4 @sm/page-card:flex-row @sm/page-card:items-center @sm/page-card:justify-between">
      <div className="flex min-w-0 items-center gap-4">
        <div className="h-16 w-16 shrink-0 overflow-hidden rounded-full">
          {thumbnailUrl ? (
            <img
              src={thumbnailUrl}
              alt={channelName}
              className="h-full w-full object-cover"
            />
          ) : (
            <div className="flex h-full w-full items-center justify-center bg-muted text-2xl font-bold text-muted-foreground">
              {channelName.charAt(0).toUpperCase()}
            </div>
          )}
        </div>
        <div className="min-w-0">
          <h2 className="truncate text-xl font-semibold">{channelName}</h2>
          {subscribedChannel && (
            <Badge variant="secondary" className="mt-1">
              {subscribedChannel.categoryName}
            </Badge>
          )}
        </div>
      </div>
      <ChannelHeaderActions
        youtubeChannelId={youtubeChannelId}
        channelName={channelName}
        subscribedChannel={subscribedChannel}
      />
    </div>
  );
}

export default ChannelPageHeader;
