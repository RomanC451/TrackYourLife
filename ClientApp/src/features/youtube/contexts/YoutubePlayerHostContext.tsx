import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { Maximize2, X } from "lucide-react";

import { Button } from "@/components/ui/button";

import VideoPlayerDialog from "../components/dialogs/VideoPlayerDialog";

type YoutubePlayerHostState = {
  videoId: string;
} | null;

type OpenYoutubePlayerInput = {
  videoId: string;
};

type YoutubePlayerHostContextValue = {
  openYoutubePlayer: (input: OpenYoutubePlayerInput) => void;
  closeYoutubePlayer: () => void;
  minimizeYoutubePlayer: () => void;
  playerState: YoutubePlayerHostState;
};

const YoutubePlayerHostContext = createContext<YoutubePlayerHostContextValue | null>(
  null,
);

function YoutubePlayerHostDialog({
  playerState,
  minimizeYoutubePlayer,
  isVisible,
}: {
  playerState: YoutubePlayerHostState;
  minimizeYoutubePlayer: () => void;
  isVisible: boolean;
}) {
  if (!playerState) {
    return null;
  }

  const handleClose = () => {
    minimizeYoutubePlayer();
  };

  return (
    <VideoPlayerDialog
      videoId={playerState.videoId}
      onClose={handleClose}
      isVisible={isVisible}
    />
  );
}

function YoutubePlayerMinimizedBar({
  onReopen,
  onStop,
}: {
  onReopen: () => void;
  onStop: () => void;
}) {
  return (
    <div className="fixed bottom-4 right-4 z-70 flex items-center gap-2 rounded-md border bg-background/95 p-2 shadow-lg backdrop-blur">
      <span className="text-xs text-muted-foreground">Player minimized</span>
      <Button size="sm" variant="secondary" onClick={onReopen}>
        <Maximize2 className="mr-1 h-4 w-4" />
        Open
      </Button>
      <Button size="sm" variant="destructive" onClick={onStop}>
        <X className="mr-1 h-4 w-4" />
        Stop
      </Button>
    </div>
  );
}

export function YoutubePlayerHostProvider({
  children,
}: {
  children: ReactNode;
}) {
  const [playerState, setPlayerState] = useState<YoutubePlayerHostState>(null);
  const [isMinimized, setIsMinimized] = useState(false);

  const openYoutubePlayer = useCallback(({ videoId }: OpenYoutubePlayerInput) => {
    setPlayerState({ videoId });
    setIsMinimized(false);
  }, []);

  const closeYoutubePlayer = useCallback(() => {
    setPlayerState(null);
    setIsMinimized(false);
  }, []);

  const minimizeYoutubePlayer = useCallback(() => {
    setIsMinimized(true);
  }, []);

  const value = useMemo<YoutubePlayerHostContextValue>(
    () => ({
      openYoutubePlayer,
      closeYoutubePlayer,
      minimizeYoutubePlayer,
      playerState,
    }),
    [closeYoutubePlayer, minimizeYoutubePlayer, openYoutubePlayer, playerState],
  );

  return (
    <YoutubePlayerHostContext.Provider value={value}>
      {children}
      {playerState && isMinimized && (
        <YoutubePlayerMinimizedBar
          onReopen={() => setIsMinimized(false)}
          onStop={closeYoutubePlayer}
        />
      )}
      {playerState && (
        <div className={isMinimized ? "pointer-events-none opacity-0" : undefined}>
          <YoutubePlayerHostDialog
            key={playerState.videoId}
            playerState={playerState}
            minimizeYoutubePlayer={value.minimizeYoutubePlayer}
            isVisible={!isMinimized}
          />
        </div>
      )}
    </YoutubePlayerHostContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useYoutubePlayerHost() {
  const context = useContext(YoutubePlayerHostContext);

  if (!context) {
    throw new Error("useYoutubePlayerHost must be used within YoutubePlayerHostProvider");
  }

  return context;
}
