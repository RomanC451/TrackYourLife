import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { BookOpen, Gamepad2, Loader2, Search, Users } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { VideoCategory, YoutubeChannelSearchResult } from "@/services/openapi";

import useAddChannelMutation from "../../mutations/useAddChannelMutation";
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
  isAdding: boolean;
}

function ChannelSearchResult({
  channel,
  onAdd,
  isAdding,
}: ChannelSearchResultProps) {
  return (
    <div className="flex items-start gap-3 rounded-lg border p-3">
      <div className="h-12 w-12 flex-shrink-0 overflow-hidden rounded-full">
        <img
          src={channel.thumbnailUrl}
          alt={channel.name}
          className="h-full w-full object-cover"
        />
      </div>
      <div className="min-w-0 flex-1">
        <h4 className="truncate font-medium">{channel.name}</h4>
        <div className="mt-0.5 flex items-center gap-1 text-xs text-muted-foreground">
          <Users className="h-3 w-3" />
          <span>{formatSubscriberCount(channel.subscriberCount)}</span>
        </div>
        <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
          {channel.description}
        </p>
      </div>
      <div className="flex flex-shrink-0 flex-col gap-1">
        <Button
          size="sm"
          variant="outline"
          className="text-xs"
          onClick={() => onAdd("Entertainment")}
          disabled={isAdding}
        >
          <Gamepad2 className="mr-1 h-3 w-3" />
          Fun
        </Button>
        <Button
          size="sm"
          variant="outline"
          className="text-xs"
          onClick={() => onAdd("Educational")}
          disabled={isAdding}
        >
          <BookOpen className="mr-1 h-3 w-3" />
          Learn
        </Button>
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
  const [addingChannelId, setAddingChannelId] = useState<string | null>(null);

  const addChannelMutation = useAddChannelMutation();

  // Debounce search query with useEffect
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setDebouncedQuery(searchQuery);
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchQuery]);

  const { data: searchResults, isLoading: isSearching } = useQuery({
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
    setAddingChannelId(channel.channelId);
    try {
      await addChannelMutation.mutateAsync({
        youtubeChannelId: channel.channelId,
        category,
        channelName: channel.name,
      });
    } finally {
      setAddingChannelId(null);
    }
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
                      isAdding={addingChannelId === channel.channelId}
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
