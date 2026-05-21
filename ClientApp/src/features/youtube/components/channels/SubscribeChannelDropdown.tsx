import type { ReactNode } from "react";
import { Link } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { ChevronDown, Loader2, Trash2, UserPlus } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";

import useAddChannelMutation from "../../mutations/useAddChannelMutation";
import useRemoveChannelMutation from "../../mutations/useRemoveChannelMutation";
import { youtubeQueryOptions } from "../../queries/youtubeQueries";
import { sortYoutubeCategoriesByDisplayOrder } from "../../youtubeListSearch";

interface SubscribeChannelDropdownProps {
  channelId: string;
  channelName: string;
  triggerClassName?: string;
  menuContentClassName?: string;
  showLabel?: boolean;
}

function SubscribeChannelDropdown({
  channelId,
  channelName,
  triggerClassName,
  menuContentClassName,
  showLabel = false,
}: SubscribeChannelDropdownProps) {
  const { data: settings, isPending: isSettingsPending } = useQuery(
    youtubeQueryOptions.settings(),
  );
  const { data: channels, isPending: isChannelsPending } = useQuery(
    youtubeQueryOptions.channels("all"),
  );

  const addChannelMutation = useAddChannelMutation();
  const removeChannelMutation = useRemoveChannelMutation();

  const categories = sortYoutubeCategoriesByDisplayOrder(
    settings?.categories ?? [],
  );
  const isSubscribed = channels?.some(
    (channel) => channel.youtubeChannelId === channelId,
  );
  const isLoading = isSettingsPending || isChannelsPending;
  const hasCategories = categories.length > 0;

  const handleAdd = (youtubeCategoryId: string, categoryName: string) => {
    addChannelMutation.mutate({
      youtubeChannelId: channelId,
      youtubeCategoryId,
      channelName,
      categoryName,
    });
  };

  const handleRemove = () => {
    removeChannelMutation.mutate({ youtubeChannelId: channelId });
  };

  let menuBody: ReactNode;
  if (isLoading) {
    menuBody = (
      <div className="flex justify-center py-2">
        <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
      </div>
    );
  } else if (!hasCategories) {
    menuBody = (
      <div className="px-2 py-1.5 text-sm text-muted-foreground">
        Add a category first.{" "}
        <Link to="/youtube/settings" className="text-primary underline">
          Settings
        </Link>
      </div>
    );
  } else {
    menuBody = categories.map((cat) => (
      <DropdownMenuItem
        key={cat.id}
        disabled={addChannelMutation.isPending}
        onSelect={() => handleAdd(cat.id, cat.name)}
      >
        {cat.name}
      </DropdownMenuItem>
    ));
  }

  if (isSubscribed) {
    return (
      <ButtonWithLoading
        type="button"
        size="sm"
        variant={triggerClassName ? "secondary" : "outline"}
        className={cn(
          "shrink-0",
          triggerClassName ??
            "border-destructive/50 text-destructive hover:bg-destructive/10 hover:text-destructive",
        )}
        onClick={handleRemove}
        disabled={removeChannelMutation.isPending}
        isLoading={removeChannelMutation.isDelayedPending}
      >
        <Trash2 className="h-4 w-4" />
        {showLabel ? <span className="ml-1.5">Unsubscribe</span> : null}
        <span className="sr-only">Unsubscribe from {channelName}</span>
      </ButtonWithLoading>
    );
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          type="button"
          size="sm"
          variant="secondary"
          className={cn("shrink-0 gap-1", triggerClassName)}
          disabled={addChannelMutation.isPending || (!hasCategories && !isLoading)}
        >
          {addChannelMutation.isPending ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <UserPlus className="h-4 w-4" />
          )}
          {showLabel ? <span>Subscribe</span> : null}
          {hasCategories ? (
            <ChevronDown className="h-3 w-3 shrink-0 opacity-70" />
          ) : null}
          <span className="sr-only">Subscribe to {channelName}</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        align="start"
        className={cn("z-[100] min-w-40", menuContentClassName)}
      >
        <DropdownMenuLabel>Subscribe to category</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {menuBody}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default SubscribeChannelDropdown;
