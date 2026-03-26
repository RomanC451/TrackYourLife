import { Suspense, useState } from "react";
import { createFileRoute, Link, Outlet, useLocation } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { ChevronRight, Loader2, Plus, Trash2 } from "lucide-react";

import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbList,
  BreadcrumbPage,
} from "@/components/ui/breadcrumb";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import CreatePlaylistDialog from "@/features/youtube/components/library/CreatePlaylistDialog";
import { useDeletePlaylistMutation } from "@/features/youtube/mutations/useLibraryPlaylistMutations";
import { youtubeQueryOptions } from "@/features/youtube/queries/youtubeQueries";

export const Route = createFileRoute(
  "/_authenticated/_sidebarPageLayout/_navbarPageLayout/youtube/library",
)({
  component: RouteComponent,
});

function playlistSubtitle(videoCount: number): string {
  if (videoCount === 0) {
    return "Add videos from Videos or Search";
  }
  const label = videoCount === 1 ? "video" : "videos";
  return `${videoCount} ${label}`;
}

/** Thumbnail for the most recently added video, with offset layers behind (playlist order is oldest → newest). */
function PlaylistLastVideoStackThumb({ thumbnailUrl }: { thumbnailUrl: string }) {
  return (
    <div className="relative h-12 w-17 shrink-0">
      <div
        className="pointer-events-none absolute inset-0 translate-x-1 translate-y-1 rounded-md border border-border/70 bg-muted/55 shadow-sm"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute inset-0 translate-x-0.5 translate-y-0.5 rounded-md border border-border/70 bg-muted/35"
        aria-hidden
      />
      <img
        src={thumbnailUrl}
        alt=""
        className="relative h-full w-full rounded-md border object-cover shadow-sm"
      />
    </div>
  );
}

function RouteComponent() {
  const [createOpen, setCreateOpen] = useState(false);
  const pathname = useLocation({ select: (loc) => loc.pathname });
  const segments = pathname.split("/").filter(Boolean);
  const isPlaylistDetail =
    segments.length >= 3 &&
    segments[0] === "youtube" &&
    segments[1] === "library";

  if (isPlaylistDetail) {
    return <Outlet />;
  }

  return (
    <PageCard>
      <PageTitle title={<Breadcrumb className="mb-4">
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbPage>Library</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>}>
        <Button type="button" onClick={() => setCreateOpen(true)}>
          <Plus className="mr-2 h-4 w-4" />
          New playlist
        </Button>
      </PageTitle>
      <CreatePlaylistDialog open={createOpen} onOpenChange={setCreateOpen} />
      <Suspense
        fallback={
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        }
      >
        <LibraryPlaylistsBody />
      </Suspense>
    </PageCard>
  );
}

function LibraryPlaylistsBody() {
  const { data: playlists } = useSuspenseQuery(youtubeQueryOptions.libraryPlaylists());
  const deleteMutation = useDeletePlaylistMutation();
  const [deleteId, setDeleteId] = useState<string | null>(null);

  if (playlists.length === 0) {
    return (
      <p className="py-8 text-center text-muted-foreground">
        No playlists yet. Create one to save videos from the Videos or Search pages.
      </p>
    );
  }

  return (
    <>
      <ul className="divide-y rounded-lg border">
        {playlists.map((p) => {
          const lastPreview =
            p.videoPreviews.length > 0
              ? p.videoPreviews[p.videoPreviews.length - 1]
              : undefined;
          return (
            <li
              key={p.id}
              className="flex items-center justify-between gap-3 px-4 py-3 first:rounded-t-lg last:rounded-b-lg"
            >
              <Link
                to="/youtube/library/$playlistId"
                params={{ playlistId: p.id }}
                className="group flex min-w-0 flex-1 items-center gap-3"
              >
                {lastPreview ? (
                  <PlaylistLastVideoStackThumb thumbnailUrl={lastPreview.thumbnailUrl} />
                ) : (
                  <div
                    className="flex h-12 w-16 shrink-0 items-center justify-center rounded-md border border-dashed bg-muted/30 text-center text-[10px] leading-tight text-muted-foreground"
                    aria-hidden
                  >
                    No videos
                  </div>
                )}
                <span className="min-w-0 flex-1">
                  <span className="block font-medium group-hover:underline">{p.name}</span>
                  <span className="block text-xs text-muted-foreground">
                    {playlistSubtitle(p.videoPreviews.length)}
                  </span>
                </span>
                <ChevronRight className="h-4 w-4 shrink-0 text-muted-foreground opacity-70 group-hover:opacity-100" />
              </Link>
              <Button
                type="button"
                variant="ghost"
                size="icon"
                className="shrink-0 text-destructive hover:text-destructive"
                onClick={() => setDeleteId(p.id)}
                aria-label="Delete playlist"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </li>
          );
        })}
      </ul>

      <AlertDialog open={deleteId !== null} onOpenChange={() => setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete this playlist?</AlertDialogTitle>
            <AlertDialogDescription>
              This removes the playlist and its saved video references. This cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={() => {
                if (deleteId) {
                  deleteMutation.mutate(deleteId, { onSuccess: () => setDeleteId(null) });
                }
              }}
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
