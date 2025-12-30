import { Trash2 } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { YoutubeChannelDto } from "@/services/openapi";

import useRemoveChannelMutation from "../../mutations/useRemoveChannelMutation";

interface ChannelCardProps {
  channel: YoutubeChannelDto;
}

function ChannelCard({ channel }: ChannelCardProps) {
  const removeChannelMutation = useRemoveChannelMutation();

  const handleDelete = () => {
    removeChannelMutation.mutate({
      id: channel.id,
      name: channel.name,
    });
  };

  const isDisabled = channel.isDeleting || channel.isLoading;

  return (
    <Card
      className={cn("transition-opacity", {
        "opacity-50": isDisabled,
      })}
    >
      <CardContent className="flex items-center gap-4 p-4">
        {/* Channel Thumbnail */}
        <div className="h-16 w-16 flex-shrink-0 overflow-hidden rounded-full">
          {channel.thumbnailUrl ? (
            <img
              src={channel.thumbnailUrl}
              alt={channel.name}
              className="h-full w-full object-cover"
            />
          ) : (
            <div className="flex h-full w-full items-center justify-center bg-muted text-2xl font-bold text-muted-foreground">
              {channel.name.charAt(0).toUpperCase()}
            </div>
          )}
        </div>

        {/* Channel Info */}
        <div className="min-w-0 flex-1">
          <h3 className="truncate text-base font-semibold">{channel.name}</h3>
          <Badge
            variant={
              channel.category === "Educational" ? "default" : "secondary"
            }
            className="mt-1"
          >
            {channel.category}
          </Badge>
        </div>

        {/* Delete Button */}
        <Button
          variant="ghost"
          size="icon"
          className="flex-shrink-0 text-destructive hover:bg-destructive/10 hover:text-destructive"
          onClick={handleDelete}
          disabled={isDisabled}
        >
          <Trash2 className="h-4 w-4" />
        </Button>
      </CardContent>
    </Card>
  );
}

export default ChannelCard;

