import { useEffect, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import {
  BookOpen,
  Gamepad2,
  Loader2,
  Search,
  Trash2,
  Users,
} from "lucide-react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { useCustomQuery } from "@/hooks/useCustomQuery";
import { cn } from "@/lib/utils";
import { VideoCategory, YoutubeChannelSearchResult } from "@/services/openapi";

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
  onAdd: (category: VideoCategory) => void;
  onRemove: () => void;
  addPendingState: MutationPendingState;
  removePendingState: MutationPendingState;
}

function ChannelSearchResult({
  channel,
  onAdd,
  onRemove,
  addPendingState,
  removePendingState,
}: ChannelSearchResultProps) {
  const isSubscribed = channel.alreadySubscribed;

  return (
    <div
      className={cn("flex items-start gap-3 rounded-lg border p-3", {
        "border-primary/30 bg-primary/5": isSubscribed,
        "opacity-60":
          addPendingState.isDelayedPending ||
          removePendingState.isDelayedPending,
      })}
    >
      <div className="h-12 w-12 shrink-0 overflow-hidden rounded-full">
        <img
          src={channel.thumbnailUrl}
          alt={channel.name}
          className="h-full w-full object-cover"
        />
      </div>
      <div className="min-w-0 flex-1">
        <div className="flex items-center gap-2">
          <h4 className="truncate font-medium">{channel.name}</h4>
          {isSubscribed && (
            <span className="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-medium text-primary">
              Subscribed
            </span>
          )}
        </div>
        <div className="mt-0.5 flex items-center gap-1 text-xs text-muted-foreground">
          <Users className="h-3 w-3" />
          <span>{formatSubscriberCount(channel.subscriberCount)}</span>
        </div>
        <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
          {channel.description}
        </p>
      </div>
      <div className="flex shrink-0 flex-col gap-1">
        {isSubscribed ? (
          <ButtonWithLoading
            size="sm"
            variant="ghost"
            className="text-xs text-destructive hover:bg-destructive/10 hover:text-destructive"
            onClick={onRemove}
            disabled={removePendingState.isPending}
            isLoading={removePendingState.isDelayedPending}
          >
            <Trash2 className="mr-1 h-3 w-3" />
            Remove
          </ButtonWithLoading>
        ) : (
          <>
            <ButtonWithLoading
              size="sm"
              variant="outline"
              className="text-xs"
              onClick={() => onAdd("Entertainment")}
              disabled={addPendingState.isPending}
              isLoading={addPendingState.isDelayedPending}
            >
              <Gamepad2 className="mr-1 h-3 w-3" />
              Fun
            </ButtonWithLoading>
            <ButtonWithLoading
              size="sm"
              variant="outline"
              className="text-xs"
              onClick={() => onAdd("Educational")}
              disabled={addPendingState.isPending}
              isLoading={addPendingState.isDelayedPending}
            >
              <BookOpen className="mr-1 h-3 w-3" />
              Learn
            </ButtonWithLoading>
          </>
        )}
      </div>
    </div>
  );
}

interface AddChannelDialogProps {
  onClose?: () => void;
}

function AddChannelDialog({ onClose }: AddChannelDialogProps) {
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedQuery, setDebouncedQuery] = useState("");

  const addChannelMutation = useAddChannelMutation();
  const removeChannelMutation = useRemoveChannelMutation();

  // Debounce search query with useEffect
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setDebouncedQuery(searchQuery);
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchQuery]);

  const {
    query: { data: searchResults },
    pendingState: { isPending: isSearching },
  } = useCustomQuery({
    ...youtubeQueryOptions.channelSearch(debouncedQuery, 10),
    enabled: debouncedQuery.length >= 2,
  });

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
    category: VideoCategory,
  ) => {
    addChannelMutation.mutateAsync({
      youtubeChannelId: channel.channelId,
      category,
      channelName: channel.name,
    });
  };

  const handleRemoveChannel = async (channel: YoutubeChannelSearchResult) => {
    removeChannelMutation.mutateAsync({
      id: channel.channelId,
      name: channel.name,
    });
  };

  return (
    <Dialog onOpenChange={handleOpenChange} defaultOpen={true}>
      <DialogContent className="max-h-[90dvh] max-w-[500px]" withoutOverlay>
        <DialogHeader>
          <DialogTitle>Add YouTube Channel</DialogTitle>
          <DialogDescription>
            Search for YouTube channels and add them to your library
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="relative">
            <Input
              placeholder="Search for channels..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pr-10"
            />
            <Search className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          </div>

          <div className="max-h-[400px] space-y-2 overflow-y-auto">
            {isSearching && (
              <div className="flex items-center justify-center py-8">
                <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
              </div>
            )}

            {!isSearching && debouncedQuery.length >= 2 && searchResults && (
              <>
                {searchResults.length === 0 ? (
                  <div className="py-8 text-center text-muted-foreground">
                    No channels found for "{debouncedQuery}"
                  </div>
                ) : (
                  searchResults.map((channel) => (
                    <ChannelSearchResult
                      key={channel.channelId}
                      channel={channel}
                      onAdd={(category) => handleAddChannel(channel, category)}
                      onRemove={() => handleRemoveChannel(channel)}
                      addPendingState={addChannelMutation.pendingState}
                      removePendingState={removeChannelMutation.pendingState}
                    />
                  ))
                )}
              </>
            )}

            {!isSearching && debouncedQuery.length < 2 && (
              <div className="py-8 text-center text-muted-foreground">
                Enter at least 2 characters to search
              </div>
            )}
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default AddChannelDialog;
