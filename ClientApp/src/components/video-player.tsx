function getYouTubeVideoId(url: string): {
  videoId: string | null;
  startTime: number | null;
} {
  try {
    const urlObj = new URL(url);
    let videoId: string | null = null;
    let startTime: number | null = null;

    // Get video ID
    if (urlObj.hostname.includes("youtube.com")) {
      if (urlObj.pathname.includes("/shorts/")) {
        // Handle YouTube Shorts URLs
        videoId = urlObj.pathname.split("/shorts/")[1];
      } else {
        videoId = urlObj.searchParams.get("v");
      }
    } else if (urlObj.hostname === "youtu.be") {
      videoId = urlObj.pathname.slice(1);
    }

    // Get start time
    const timeParam = urlObj.searchParams.get("t");
    if (timeParam) {
      // Convert time parameter to seconds
      startTime = parseInt(timeParam, 10);
    }

    return { videoId, startTime };
  } catch {
    return { videoId: null, startTime: null };
  }
}

function VideoPlayer({
  url,
  onLoaded,
  onError,
}: {
  url: string;
  onLoaded?: () => void;
  onError?: () => void;
}) {
  const { videoId, startTime } = getYouTubeVideoId(url);

  if (videoId) {
    const embedUrl = `https://www.youtube.com/embed/${videoId}${startTime ? `?start=${startTime}` : ""}`;
    return (
      <iframe
        src={embedUrl}
        className="aspect-video w-full rounded-md"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowFullScreen
        onLoad={onLoaded}
        onError={onError}
      />
    );
  }

  return (
    <video
      src={url}
      controls
      className="w-full rounded-md"
      style={{ maxHeight: "300px" }}
      onLoadedData={onLoaded}
      onError={onError}
    />
  );
}

export default VideoPlayer;
