export const preloadImage = (src: string): Promise<void> => {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve();
    img.onerror = () => reject(new Error(`Failed to load image: ${src}`));
    img.src = src;
  });
};

// Helper function to preload multiple images
export const preloadImages = async (urls: string[]): Promise<void> => {
  const validUrls = urls.filter((url) => url && url.trim() !== "");
  if (validUrls.length === 0) return;

  try {
    await Promise.allSettled(validUrls.map((url) => preloadImage(url)));
  } catch (error) {
    // Silently fail - image preloading shouldn't break the app
    console.warn("Some images failed to preload:", error);
  }
};
