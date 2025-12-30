import { useSuspenseQuery } from "@tanstack/react-query";

import { VideoCategory } from "@/services/openapi";

import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import ChannelCard from "./ChannelCard";

interface ChannelsListProps {
  category?: VideoCategory | null;
}

function ChannelsList({ category }: ChannelsListProps) {
  const { data: channels } = useSuspenseQuery(
    youtubeQueryOptions.channels(category)
  );

  if (channels.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg text-muted-foreground">No channels found</p>
        <p className="mt-2 text-sm text-muted-foreground">
          Add channels to start watching their latest videos
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-4 @xl/page-card:grid-cols-2 @4xl/page-card:grid-cols-3">
      {channels.map((channel) => (
        <ChannelCard key={channel.id} channel={channel} />
      ))}
    </div>
  );
}

export default ChannelsList;

