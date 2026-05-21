import { FolderInput, Loader2, Trash2 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { YoutubeChannelDto } from "@/services/openapi";

import SubscribeChannelDropdown from "../channels/SubscribeChannelDropdown";
import useMoveChannelToCategoryMutation from "../../mutations/useMoveChannelToCategoryMutation";
import useRemoveChannelMutation from "../../mutations/useRemoveChannelMutation";
import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import { sortYoutubeCategoriesByDisplayOrder } from "../../youtubeListSearch";

interface ChannelHeaderActionsProps {
  youtubeChannelId: string;
  channelName: string;
  subscribedChannel: YoutubeChannelDto | undefined;
}

function ChannelHeaderActions({
  youtubeChannelId,
  channelName,
  subscribedChannel,
}: ChannelHeaderActionsProps) {
  const { data: settings } = useQuery(youtubeQueryOptions.settings());
  const removeChannelMutation = useRemoveChannelMutation();
  const moveChannelMutation = useMoveChannelToCategoryMutation();

  const categories = sortYoutubeCategoriesByDisplayOrder(
    settings?.categories ?? [],
  );

  if (!subscribedChannel) {
    return (
      <SubscribeChannelDropdown
        channelId={youtubeChannelId}
        channelName={channelName}
        showLabel
      />
    );
  }

  const otherCategories = categories.filter(
    (c) => c.id !== subscribedChannel.youtubeCategoryId,
  );

  const isDisabled =
    subscribedChannel.isDeleting ||
    subscribedChannel.isLoading ||
    moveChannelMutation.isPending;

  const handleDelete = () => {
    removeChannelMutation.mutate({
      youtubeChannelId: subscribedChannel.youtubeChannelId,
    });
  };

  const handleMove = (targetCategoryId: string) => {
    moveChannelMutation.mutate({
      youtubeChannelId: subscribedChannel.youtubeChannelId,
      targetYoutubeCategoryId: targetCategoryId,
    });
  };

  return (
    <div className="flex shrink-0 flex-wrap items-center gap-1">
      {otherCategories.length > 0 && (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              type="button"
              variant="outline"
              size="sm"
              disabled={isDisabled}
              title="Move to category"
            >
              {moveChannelMutation.isPending ? (
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              ) : (
                <FolderInput className="mr-2 h-4 w-4" />
              )}
              Move
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="min-w-40">
            <div className="px-2 py-1.5 text-xs font-medium text-muted-foreground">
              Move to
            </div>
            {otherCategories.map((cat) => (
              <DropdownMenuItem
                key={cat.id}
                disabled={moveChannelMutation.isPending}
                onSelect={() => handleMove(cat.id)}
              >
                <span className="truncate">{cat.name}</span>
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      )}
      <Button
        type="button"
        variant="outline"
        size="sm"
        className="text-destructive hover:text-destructive"
        onClick={handleDelete}
        disabled={isDisabled}
        title="Remove channel"
      >
        <Trash2 className="mr-2 h-4 w-4" />
        Remove
      </Button>
    </div>
  );
}

export default ChannelHeaderActions;
