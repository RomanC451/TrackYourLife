import { Link } from "@tanstack/react-router";
import { FolderInput, Loader2, Star, Trash2 } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";
import type { YoutubeCategorySettingsDto } from "@/services/openapi";
import { YoutubeChannelDto } from "@/services/openapi";

import useMoveChannelToCategoryMutation from "../../mutations/useMoveChannelToCategoryMutation";
import useRemoveChannelMutation from "../../mutations/useRemoveChannelMutation";
import useSetChannelFavoriteMutation from "../../mutations/useSetChannelFavoriteMutation";

interface ChannelCardProps {
  channel: YoutubeChannelDto;
  categories: YoutubeCategorySettingsDto[];
}

function ChannelCard({ channel, categories }: ChannelCardProps) {
  const removeChannelMutation = useRemoveChannelMutation();
  const moveChannelMutation = useMoveChannelToCategoryMutation();
  const setFavoriteMutation = useSetChannelFavoriteMutation();

  const otherCategories = categories.filter(
    (c) => c.id !== channel.youtubeCategoryId,
  );

  const handleDelete = () => {
    removeChannelMutation.mutate({
      youtubeChannelId: channel.youtubeChannelId,
    });
  };

  const handleMove = (targetCategoryId: string) => {
    moveChannelMutation.mutate({
      youtubeChannelId: channel.youtubeChannelId,
      targetYoutubeCategoryId: targetCategoryId,
    });
  };

  const handleToggleFavorite = () => {
    setFavoriteMutation.mutate({
      youtubeChannelId: channel.youtubeChannelId,
      isFavorite: !channel.isFavorite,
    });
  };

  const isDisabled =
    channel.isDeleting ||
    channel.isLoading ||
    moveChannelMutation.isPending ||
    setFavoriteMutation.isPending;

  return (
    <Card
      className={cn("transition-opacity", {
        "opacity-50": isDisabled,
      })}
    >
      <CardContent className="flex items-center gap-4 p-4">
        <Link
          to="/youtube/channels/$youtubeChannelId"
          params={{ youtubeChannelId: channel.youtubeChannelId }}
          className="flex min-w-0 flex-1 items-center gap-4 transition-opacity hover:opacity-80"
        >
          <div className="h-16 w-16 shrink-0 overflow-hidden rounded-full">
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

          <div className="min-w-0 flex-1">
            <h3 className="truncate text-base font-semibold">{channel.name}</h3>
            <Badge variant="secondary" className="mt-1">
              {channel.categoryName}
            </Badge>
          </div>
        </Link>

        <div className="flex shrink-0 items-center gap-1">
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className={cn("h-9 w-9", {
              "text-yellow-500 hover:text-yellow-600": channel.isFavorite,
            })}
            disabled={isDisabled}
            title={
              channel.isFavorite
                ? "Remove from favorites"
                : "Add to favorites"
            }
            onClick={(event) => {
              event.preventDefault();
              event.stopPropagation();
              handleToggleFavorite();
            }}
          >
            {setFavoriteMutation.isPending ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Star
                className={cn("h-4 w-4", {
                  "fill-current": channel.isFavorite,
                })}
              />
            )}
            <span className="sr-only">Toggle favorite</span>
          </Button>
          {otherCategories.length > 0 && (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="h-9 w-9"
                  disabled={isDisabled}
                  title="Move to category"
                >
                  {moveChannelMutation.isPending ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <FolderInput className="h-4 w-4" />
                  )}
                  <span className="sr-only">Move to category</span>
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
            variant="ghost"
            size="icon"
            className="h-9 w-9 text-destructive hover:bg-destructive/10 hover:text-destructive"
            onClick={handleDelete}
            disabled={isDisabled}
            title="Remove channel"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}

export default ChannelCard;
