import { useEffect, useState, type FormEvent } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useCustomQuery } from "@/hooks/useCustomQuery";

import type { GuitarTabContent } from "../api/guitarTabsApi";
import { youtubePlayerSrc, youtubeVideoId } from "../lib/youtubeUrl";
import { useSetGuitarTabYoutubeMutation } from "../mutations/useSavedGuitarTabMutations";
import { guitarTabsQueryOptions } from "../queries/guitarTabsQueries";

export function GuitarTabYoutubePlayer({
  songName,
  artistName,
  suggestedUrl,
  savedUrl,
  pending,
  onSave,
}: {
  songName: string;
  artistName: string;
  suggestedUrl: string | null;
  savedUrl: string | null;
  pending: boolean;
  onSave: (youtubeUrl: string | null) => void;
}) {
  const activeUrl = savedUrl ?? suggestedUrl;
  const embedUrl = youtubePlayerSrc(
    activeUrl,
    window.location.hostname,
    import.meta.env.VITE_APP_ORIGIN,
  );
  const [value, setValue] = useState(savedUrl ?? suggestedUrl ?? "");
  const [invalid, setInvalid] = useState(false);

  useEffect(() => {
    setValue(savedUrl ?? suggestedUrl ?? "");
    setInvalid(false);
  }, [savedUrl, suggestedUrl]);

  const submit = (event: FormEvent) => {
    event.preventDefault();
    const next = value.trim();
    if (next.length === 0) {
      onSave(null);
      return;
    }

    if (!youtubeVideoId(next)) {
      setInvalid(true);
      return;
    }

    setInvalid(false);
    onSave(next);
  };

  return (
    <section className="flex flex-col gap-3" aria-label="Song player">
      {embedUrl ? (
        <iframe
          src={embedUrl}
          title={`${songName} by ${artistName}`}
          className="aspect-video w-full overflow-hidden rounded-md"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
          referrerPolicy="strict-origin-when-cross-origin"
          allowFullScreen
        />
      ) : (
        <p className="text-sm text-muted-foreground">
          This tab has no YouTube video. Paste a link to play the song while you
          read.
        </p>
      )}
      {embedUrl && !savedUrl && suggestedUrl && (
        <p className="text-sm text-muted-foreground">
          Playing the video linked on the tab.
        </p>
      )}
      <form className="flex flex-col gap-2" onSubmit={submit}>
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
          <p className="text-sm text-destructive">Enter a YouTube video link.</p>
        )}
        <div className="flex gap-2">
          <Button type="submit" size="sm" disabled={pending}>
            Save video
          </Button>
          {savedUrl && (
            <Button
              type="button"
              size="sm"
              variant="outline"
              disabled={pending}
              onClick={() => onSave(null)}
            >
              Remove video
            </Button>
          )}
        </div>
      </form>
    </section>
  );
}

function GuitarTabYoutube({ tab }: { tab: GuitarTabContent }) {
  const { query: savedQuery } = useCustomQuery(
    guitarTabsQueryOptions.savedTab(tab.tabUrl),
  );
  const mutation = useSetGuitarTabYoutubeMutation();

  return (
    <GuitarTabYoutubePlayer
      songName={tab.songName}
      artistName={tab.artistName}
      suggestedUrl={tab.youtubeUrl}
      savedUrl={savedQuery.data?.youtubeUrl ?? null}
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

export default GuitarTabYoutube;
