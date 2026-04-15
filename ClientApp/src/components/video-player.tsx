function VideoPlayer({
  url,
  onLoaded,
  onError,
}: {
  url: string;
  onLoaded?: () => void;
  onError?: () => void;
}) {
  return (
    <iframe
      src={url}
      title="External video player"
      className="aspect-video w-full rounded-md"
      allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
      allowFullScreen
      onLoad={onLoaded}
      onError={onError}
    />
  );
}

export default VideoPlayer;
