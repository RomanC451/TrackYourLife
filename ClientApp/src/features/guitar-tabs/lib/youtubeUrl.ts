const videoIdPattern =
  /(?:youtube\.com\/(?:watch\?(?:[^#]*&)?v=|embed\/|shorts\/|live\/)|youtu\.be\/)([A-Za-z0-9_-]{11})/i;

export function youtubeVideoId(url: string | null | undefined) {
  const match = url?.trim().match(videoIdPattern);
  return match?.[1] ?? null;
}

export function youtubeEmbedUrl(url: string | null | undefined) {
  const id = youtubeVideoId(url);
  return id ? `https://www.youtube.com/embed/${id}` : null;
}

const ipv4Pattern = /^(?:\d{1,3}\.){3}\d{1,3}$/;

export function youtubePlayerSrc(
  url: string | null | undefined,
  pageHostname: string,
  appOrigin?: string | null,
) {
  const id = youtubeVideoId(url);
  if (!id) {
    return null;
  }

  const direct = `https://www.youtube.com/embed/${id}`;
  const origin = appOrigin?.trim().replace(/\/$/, "");
  const pageHostIsIp = ipv4Pattern.test(pageHostname) || pageHostname.includes(":");
  if (!origin || !pageHostIsIp) {
    return direct;
  }

  return `${origin}/youtube-embed.html?v=${encodeURIComponent(id)}`;
}
