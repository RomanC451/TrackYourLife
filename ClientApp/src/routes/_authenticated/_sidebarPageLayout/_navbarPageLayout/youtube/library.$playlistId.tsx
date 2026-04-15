import { Suspense, useState } from "react";
import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Loader2, Pencil, Trash2 } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import EditPlaylistDialog from "@/features/youtube/components/library/EditPlaylistDialog";
import PlaylistVideoRow from "@/features/youtube/components/library/PlaylistVideoRow";
import { useYoutubePlayerHost } from "@/features/youtube/contexts/YoutubePlayerHostContext";
import usePlayVideoMutation from "@/features/youtube/mutations/usePlayVideoMutation";
import {
  useDeletePlaylistMutation,
  useRemoveVideoFromPlaylistMutation,
} from "@/features/youtube/mutations/useLibraryPlaylistMutations";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";
import type { YoutubePlaylistVideoItemDto } from "@/services/openapi";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/library/$playlistId",
)({
  component: RouteComponent,
});

function RouteComponent() {
  const { playlistId } = Route.useParams();

  return (
    <PageCard>
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <PlaylistDetail playlistId={playlistId} />
      </Suspense>
    </PageCard>
  );
}

function PlaylistVideosSection({
  videos,
  onWatch,
  onRemoveVideo,
  playVideoPending,
  removeVideoPending,
}: {
  videos: YoutubePlaylistVideoItemDto[];
  onWatch: (videoId: string) => void;
  onRemoveVideo: (videoId: string) => void;
  playVideoPending: boolean;
  removeVideoPending: boolean;
}) {
  if (videos.length === 0) {
    return (
      <p className="py-8 text-center text-muted-foreground">
        No videos in this playlist yet. Add some from the Videos or Search pages.
      </p>
    );
  }

  return (
    <section className="space-y-4" aria-label="Playlist videos">
      <div className="flex flex-col gap-4">
        {videos.map((v) => (
          <PlaylistVideoRow
            key={v.id}
            item={v}
            preview={v.videoPreview}
            onWatch={onWatch}
            onRemove={onRemoveVideo}
            playVideoPending={playVideoPending}
            removeVideoPending={removeVideoPending}
          />
        ))}
      </div>
    </section>
  );
}

function PlaylistDetail({ playlistId }: { playlistId: string }) {
  const navigate = useNavigate();
  const { data: playlist } = useSuspenseQuery(
    youtubeQueryOptions.libraryPlaylist(playlistId),
  );
  const [editOpen, setEditOpen] = useState(false);
  const deleteMutation = useDeletePlaylistMutation();
  const removeVideoMutation = useRemoveVideoFromPlaylistMutation();
  const playVideoMutation = usePlayVideoMutation();
  const { openYoutubePlayer } = useYoutubePlayerHost();

  const handleDeletePlaylist = () => {
    deleteMutation.mutate(playlistId, {
      onSuccess: () => {
        navigate({ to: "/youtube/library" });
      },
    });
  };

  const handleWatch = (videoId: string) => {
    playVideoMutation.mutate(videoId, {
      onSuccess: () => {
        openYoutubePlayer({
          videoId,
        });
      },
    });
  };

  return (
    <>
      <PageTitle title={<Breadcrumb className="mb-4">
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link to="/youtube/library">Library</Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem className="min-w-0 max-w-full">
            <BreadcrumbPage className="truncate">{playlist.name}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>}>
        <div className="flex flex-wrap gap-2">
          <Button type="button" variant="outline" size="sm" onClick={() => setEditOpen(true)}>
            <Pencil className="mr-2 h-4 w-4" />
            Rename
          </Button>
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="text-destructive hover:text-destructive"
            onClick={handleDeletePlaylist}
            disabled={deleteMutation.isPending}
          >
            <Trash2 className="mr-2 h-4 w-4" />
            Delete
          </Button>
        </div>
      </PageTitle >

      <EditPlaylistDialog
        open={editOpen}
        onOpenChange={setEditOpen}
        playlistId={playlistId}
        initialName={playlist.name}
      />

      <PlaylistVideosSection
        videos={playlist.videos ?? []}
        onWatch={handleWatch}
        onRemoveVideo={(videoId) =>
          removeVideoMutation.mutate({ playlistId, videoId })
        }
        playVideoPending={playVideoMutation.isPending}
        removeVideoPending={removeVideoMutation.isPending}
      />
    </>
  );
}
