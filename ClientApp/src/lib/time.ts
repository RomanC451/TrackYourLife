export function formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60)
  
    if (hours > 0) {
      return `${hours} h  ${minutes % 60} min`
    }
  
    return `${minutes} min`
  }

export function formatDurationMs(ms: number): string {
  const minutes = Math.floor(ms / 60000);
  return formatDuration(minutes);
}