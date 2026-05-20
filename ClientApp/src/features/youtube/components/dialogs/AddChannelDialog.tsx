import { useEffect, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { ChevronDown, Loader2, Search, Trash2, Users } from "lucide-react";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { cn } from "@/lib/utils";
import type { YoutubeCategorySettingsDto } from "@/services/openapi";
import { YoutubeChannelSearchResult } from "@/services/openapi";

import useAddChannelMutation from "../../mutations/useAddChannelMutation";
import useRemoveChannelMutation from "../../mutations/useRemoveChannelMutation";
import { youtubeQueryOptions } from "../../queries/youtubeQueries";

function formatSubscriberCount(count: number): string {
  if (count >= 1000000) {
    return `${(count / 1000000).toFixed(1)}M subscribers`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K subscribers`;
  }
  return `${count} subscribers`;
}

interface ChannelSearchResultProps {
  channel: YoutubeChannelSearchResult;
  categories: YoutubeCategorySettingsDto[];
  onAdd: (youtubeCategoryId: string, categoryName: string) => void;
  onRemove: () => void;
  addPendingState: MutationPendingState;
  removePendingState: MutationPendingState;
}

function ChannelSearchResult({
  channel,
  categories,
  onAdd,
  onRemove,
  addPendingState,
  removePendingState,
}: ChannelSearchResultProps) {
  const isSubscribed = channel.alreadySubscribed;

  return (
    <div
      className={cn(
        "flex min-w-0 items-center gap-3 overflow-hidden rounded-lg border p-3",
        {
          "border-primary/30 bg-primary/5": isSubscribed,
          "opacity-60":
            addPendingState.isDelayedPending ||
            removePendingState.isDelayedPending,
        },
      )}
    >
      <div className="h-12 w-12 shrink-0 overflow-hidden rounded-full">
        <img
          src={channel.thumbnailUrl}
          alt={channel.name}
          className="h-full w-full object-cover"
        />
      </div>
      <div className="min-w-0 flex-1">
        <h4 className="truncate font-medium">{channel.name}</h4>
        <div className="mt-0.5 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs text-muted-foreground">
          <span className="flex min-w-0 items-center gap-1">
            <Users className="h-3 w-3 shrink-0" />
            <span>{formatSubscriberCount(channel.subscriberCount)}</span>
          </span>
          {isSubscribed && (
            <span className="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-medium text-primary">
              Subscribed
            </span>
          )}
        </div>
        <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
          {channel.description}
        </p>
      </div>
      <div className="flex shrink-0 flex-col gap-1">
        {isSubscribed ? (
          <ButtonWithLoading
            size="sm"
            variant="outline"
            className="border-destructive/50 text-xs text-destructive hover:bg-destructive/10 hover:text-destructive"
            onClick={onRemove}
            disabled={removePendingState.isPending}
            isLoading={removePendingState.isDelayedPending}
          >
            <Trash2 className="mr-1 h-3 w-3" />
            Remove
          </ButtonWithLoading>
        ) : (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                type="button"
                size="sm"
                variant="outline"
                className="gap-1 text-xs"
                disabled={addPendingState.isPending}
              >
                {addPendingState.isDelayedPending ? (
                  <Loader2 className="h-3 w-3 shrink-0 animate-spin" />
                ) : null}
                Subscribe
                <ChevronDown className="h-3 w-3 shrink-0 opacity-70" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="z-[100] min-w-40">
              {categories.map((cat) => (
                <DropdownMenuItem
                  key={cat.id}
                  disabled={addPendingState.isPending}
                  onSelect={() => onAdd(cat.id, cat.name)}
                >
                  {cat.name}
                </DropdownMenuItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>
    </div>
  );
}

interface AddChannelDialogProps {
  onClose?: () => void;
  categories: YoutubeCategorySettingsDto[];
  defaultYoutubeCategoryId: string;
}

function AddChannelDialog({
  onClose,
  categories,
  defaultYoutubeCategoryId,
}: AddChannelDialogProps) {
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedQuery, setDebouncedQuery] = useState("");

  const addChannelMutation = useAddChannelMutation();
  const removeChannelMutation = useRemoveChannelMutation();

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setDebouncedQuery(searchQuery);
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchQuery]);

  const {
    query: { data: searchResults, isFetching: isSearching },
  } = useCustomQuery({
    ...youtubeQueryOptions.channelSearch(debouncedQuery, 10),
    enabled: debouncedQuery.length >= 2,
  });

  const isWaitingForDebounce =
    searchQuery.length >= 2 && searchQuery !== debouncedQuery;
  const isLoadingSearch = isSearching || isWaitingForDebounce;
  const showSearchResults =
    debouncedQuery.length >= 2 && searchResults != null;

  const handleOpenChange = (open: boolean) => {
    if (!open) {
      if (onClose) {
        onClose();
      } else {
        navigate({ to: "/youtube/channels" });
      }
    }
  };

  const handleAddChannel = async (
    channel: YoutubeChannelSearchResult,
    youtubeCategoryId: string,
    categoryName: string,
  ) => {
    await addChannelMutation.mutateAsync({
      youtubeChannelId: channel.channelId,
      youtubeCategoryId,
      channelName: channel.name,
      categoryName,
    });
  };

  const handleRemoveChannel = async (channel: YoutubeChannelSearchResult) => {
    await removeChannelMutation.mutateAsync({
      youtubeChannelId: channel.channelId,
    });
  };

  const hasCategories = categories.length > 0;

  return (
    <Dialog onOpenChange={handleOpenChange} defaultOpen={true}>
      <DialogContent className="max-h-[90dvh] max-w-[500px]" withoutOverlay>
        <DialogHeader>
          <DialogTitle>Add YouTube Channel</DialogTitle>
          <DialogDescription>
            Search for YouTube channels and add them to your library
            {hasCategories && defaultYoutubeCategoryId
              ? " (default category follows your current tab)."
              : null}
          </DialogDescription>
        </DialogHeader>

        <div className="min-w-0 space-y-4">
          {!hasCategories ? (
            <p className="text-sm text-muted-foreground">
              Create at least one category in YouTube settings before adding
              channels.
            </p>
          ) : (
            <>
              <div className="relative">
                <Input
                  placeholder="Search for channels..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pr-10"
                />
                {isLoadingSearch ? (
                  <Loader2 className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 animate-spin text-muted-foreground" />
                ) : (
                  <Search className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                )}
              </div>

              {showSearchResults && (
                <ScrollArea className="h-[400px]">
                  <div className="min-w-0 space-y-2 pr-4">
                    {searchResults.length === 0 ? (
                      <div className="py-8 text-center text-muted-foreground">
                        No channels found for "{debouncedQuery}"
                      </div>
                    ) : (
                      searchResults.map((channel) => (
                        <ChannelSearchResult
                          key={channel.channelId}
                          channel={channel}
                          categories={categories}
                          onAdd={(youtubeCategoryId, categoryName) =>
                            handleAddChannel(
                              channel,
                              youtubeCategoryId,
                              categoryName,
                            )
                          }
                          onRemove={() => handleRemoveChannel(channel)}
                          addPendingState={addChannelMutation.pendingState}
                          removePendingState={
                            removeChannelMutation.pendingState
                          }
                        />
                      ))
                    )}
                  </div>
                </ScrollArea>
              )}
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default AddChannelDialog;
