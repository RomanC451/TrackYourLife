import { useEffect, useRef, useState, type FormEvent } from "react";
import { ChevronDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { useCustomQuery } from "@/hooks/useCustomQuery";

import type { GuitarTabContent } from "../api/guitarTabsApi";
import { applyRemoteVideoState } from "../lib/playbackSession";
import {
  listenToPlayer,
  playerCommandType,
  playerFrameUrl,
  playerResetType,
  postPlayerCommand,
  postPlayerReset,
  readRemotePlaying,
} from "../lib/youtubePlayerBridge";
import {
  setGuitarTabPlaybackSettings,
  useGuitarTabPlaybackSettings,
} from "../lib/playbackSettings";
import { rememberVideoDuration } from "../lib/videoDuration";
import { youtubePlayerSrc, youtubeVideoId } from "../lib/youtubeUrl";
import { useSetGuitarTabYoutubeMutation } from "../mutations/useSavedGuitarTabMutations";
import { guitarTabsQueryOptions } from "../queries/guitarTabsQueries";

export function GuitarTabYoutubePlayer({
  songName,
  artistName,
  suggestedUrl,
  savedUrl,
  canSaveVideo,
}: {
  songName: string;
  artistName: string;
  suggestedUrl: string | null;
  savedUrl: string | null;
  canSaveVideo: boolean;
}) {
  const frameRef = useRef<HTMLIFrameElement>(null);
  const activeUrl = savedUrl ?? suggestedUrl;
  const embedTarget = youtubePlayerSrc(
    activeUrl,
    window.location.hostname,
    import.meta.env.VITE_APP_ORIGIN,
  );
  const embedUrl = embedTarget
    ? playerFrameUrl(embedTarget, window.location.origin)
    : null;

  useEffect(() => {
    const onCommand = (event: Event) => {
      const frame = frameRef.current;
      if (!frame) {
        return;
      }

      postPlayerCommand(
        frame,
        (event as CustomEvent<{ playing: boolean }>).detail.playing,
      );
    };
    const onMessage = (event: MessageEvent) => {
      const frame = frameRef.current;
      if (!frame || event.source !== frame.contentWindow) {
        return;
      }

      const playing = readRemotePlaying(event.data);
      if (playing == null) {
        return;
      }

      applyRemoteVideoState(playing);
    };

    const onReset = () => {
      const frame = frameRef.current;
      if (!frame) {
        return;
      }

      postPlayerReset(frame);
    };

    window.addEventListener(playerCommandType, onCommand);
    window.addEventListener(playerResetType, onReset);
    window.addEventListener("message", onMessage);
    return () => {
      window.removeEventListener(playerCommandType, onCommand);
      window.removeEventListener(playerResetType, onReset);
      window.removeEventListener("message", onMessage);
    };
  }, [embedUrl]);

  useEffect(() => {
    rememberVideoDuration(youtubeVideoId(activeUrl));
  }, [activeUrl]);

  return (
    <section className="flex flex-col gap-3" aria-label="Song player">
      {embedUrl ? (
        <iframe
          ref={frameRef}
          src={embedUrl}
          title={`${songName} by ${artistName}`}
          onLoad={(event) => {
            if (!event.currentTarget.src.includes("youtube-embed.html")) {
              listenToPlayer(event.currentTarget);
            }
          }}
          className="aspect-video w-full overflow-hidden rounded-md"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
          referrerPolicy="strict-origin-when-cross-origin"
          allowFullScreen
        />
      ) : canSaveVideo ? (
        <p className="text-sm text-muted-foreground">
          This tab has no YouTube video. Paste a link to play the song while you
          read.
        </p>
      ) : (
        <p className="text-sm text-muted-foreground">
          Add this song to Favorites or a playlist to save a video.
        </p>
      )}
      {embedUrl && !savedUrl && suggestedUrl && (
        <p className="text-sm text-muted-foreground">
          Playing the video linked on the tab.
        </p>
      )}
    </section>
  );
}

export function GuitarTabAudioButton({
  suggestedUrl,
  savedUrl,
  pending,
  onSave,
}: {
  suggestedUrl: string | null;
  savedUrl: string | null;
  pending: boolean;
  onSave: (youtubeUrl: string | null) => void;
}) {
  const [value, setValue] = useState(savedUrl ?? suggestedUrl ?? "");
  const [invalid, setInvalid] = useState(false);
  const [editing, setEditing] = useState(false);

  useEffect(() => {
    setValue(savedUrl ?? suggestedUrl ?? "");
    setInvalid(false);
  }, [savedUrl, suggestedUrl]);

  const openEditor = () => {
    setValue(savedUrl ?? suggestedUrl ?? "");
    setInvalid(false);
    setEditing(true);
  };

  const submit = (event: FormEvent) => {
    event.preventDefault();
    const next = value.trim();
    if (next.length === 0) {
      onSave(null);
      setEditing(false);
      return;
    }

    if (!youtubeVideoId(next)) {
      setInvalid(true);
      return;
    }

    setInvalid(false);
    onSave(next);
    setEditing(false);
  };

  const settings = useGuitarTabPlaybackSettings();

  const editor = (
    <Dialog open={editing} onOpenChange={setEditing}>
      <DialogContent>
        <form onSubmit={submit}>
          <DialogHeader>
            <DialogTitle>{savedUrl ? "Edit audio" : "Add audio"}</DialogTitle>
            <DialogDescription>
              Paste a YouTube link for this song.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-2 py-4">
            <Input
              aria-label="YouTube URL"
              placeholder="https://www.youtube.com/watch?v="
              value={value}
              onChange={(event) => {
                setValue(event.target.value);
                setInvalid(false);
              }}
            />
            {invalid && (
              <p className="text-sm text-destructive">
                Enter a YouTube video link.
              </p>
            )}
          </div>
          <DialogFooter>
            <Button type="submit" disabled={pending}>
              Save
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );

  if (!savedUrl) {
    return (
      <>
        <Button
          type="button"
          variant="outline"
          disabled={pending}
          onClick={openEditor}
        >
          Add audio
        </Button>
        {editor}
      </>
    );
  }

  return (
    <>
      <DropdownMenu modal={false}>
        <DropdownMenuTrigger asChild>
          <Button type="button" variant="outline" disabled={pending}>
            Audio
            <ChevronDown />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuCheckboxItem
            checked={settings.syncVideo}
            onCheckedChange={(checked) =>
              setGuitarTabPlaybackSettings({ syncVideo: checked === true })
            }
          >
            Sync audio
          </DropdownMenuCheckboxItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onSelect={() => {
              window.setTimeout(openEditor, 0);
            }}
          >
            Edit audio
          </DropdownMenuItem>
          <DropdownMenuItem
            onSelect={() => {
              setEditing(false);
              onSave(null);
            }}
          >
            Remove audio
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
      {editor}
    </>
  );
}

export function GuitarTabAudioActions({ tab }: { tab: GuitarTabContent }) {
  const { query: savedQuery } = useCustomQuery(
    guitarTabsQueryOptions.savedTab(tab.tabUrl),
  );
  const mutation = useSetGuitarTabYoutubeMutation();
  const library = savedQuery.data;
  const canSave = Boolean(
    library && (library.isFavorite || library.playlistIds.length > 0),
  );

  if (!canSave) {
    return null;
  }

  return (
    <GuitarTabAudioButton
      suggestedUrl={tab.youtubeUrl}
      savedUrl={library?.youtubeUrl ?? null}
      pending={mutation.isPending}
      onSave={(youtubeUrl) => {
        mutation.mutate({
          tabUrl: tab.tabUrl,
          songName: tab.songName,
          artistName: tab.artistName,
          type: tab.type,
          youtubeUrl,
        });
      }}
    />
  );
}

function GuitarTabYoutube({ tab }: { tab: GuitarTabContent }) {
  const { query: savedQuery } = useCustomQuery(
    guitarTabsQueryOptions.savedTab(tab.tabUrl),
  );
  const library = savedQuery.data;
  const canSaveVideo = Boolean(
    library && (library.isFavorite || library.playlistIds.length > 0),
  );

  return (
    <GuitarTabYoutubePlayer
      songName={tab.songName}
      artistName={tab.artistName}
      suggestedUrl={tab.youtubeUrl}
      savedUrl={library?.youtubeUrl ?? null}
      canSaveVideo={canSaveVideo}
    />
  );
}

export default GuitarTabYoutube;
