/**
 * Utility functions to manage body scroll state
 * Used to prevent background scrolling when modals/dialogs are open
 */

let scrollLockCount = 0;
let originalOverflow: string | null = null;
let originalPaddingRight: string | null = null;

/**
 * Disable body scrolling
 * Can be called multiple times safely - will only re-enable when all locks are released
 */
export function disableBodyScroll(): void {
  scrollLockCount++;

  if (scrollLockCount === 1) {
    // Only apply changes on first lock
    const body = document.body;
    const scrollbarWidth =
      window.innerWidth - document.documentElement.clientWidth;

    originalOverflow = body.style.overflow;
    originalPaddingRight = body.style.paddingRight;

    body.style.overflow = "hidden";

    // Add padding to prevent layout shift when scrollbar disappears
    if (scrollbarWidth > 0) {
      body.style.paddingRight = `${scrollbarWidth}px`;
    }
  }
}

/**
 * Enable body scrolling
 * Only re-enables when all locks are released
 */
export function enableBodyScroll(): void {
  scrollLockCount = Math.max(0, scrollLockCount - 1);

  if (scrollLockCount === 0) {
    // Only restore original state when all locks are released
    const body = document.body;

    if (originalOverflow !== null) {
      body.style.overflow = originalOverflow;
      originalOverflow = null;
    }

    if (originalPaddingRight !== null) {
      body.style.paddingRight = originalPaddingRight;
      originalPaddingRight = null;
    }
  }
}

/**
 * Force reset body scroll state
 * Use this in cleanup scenarios where you need to ensure scroll is restored
 */
export function resetBodyScroll(): void {
  scrollLockCount = 0;
  const body = document.body;

  body.style.overflow = "";
  body.style.paddingRight = "";

  originalOverflow = null;
  originalPaddingRight = null;
}
