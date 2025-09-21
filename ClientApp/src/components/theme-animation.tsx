import { useEffect, useRef } from "react";
import { flushSync } from "react-dom";

const isBrowser = typeof window !== "undefined";

// Inject base CSS for view transitions
const injectBaseStyles = () => {
  if (isBrowser) {
    const styleId = "theme-switch-base-style";
    if (!document.getElementById(styleId)) {
      const style = document.createElement("style");
      style.id = styleId;
      const isHighResolution =
        window.innerWidth >= 3000 || window.innerHeight >= 2000;

      style.textContent = `
        ::view-transition-old(root),
        ::view-transition-new(root) {
          animation: none;
          mix-blend-mode: normal;
          ${isHighResolution ? "transform: translateZ(0);" : ""}
        }
        
        ${
          isHighResolution
            ? `
        ::view-transition-group(root),
        ::view-transition-image-pair(root),
        ::view-transition-old(root),
        ::view-transition-new(root) {
          backface-visibility: hidden;
          perspective: 1000px;
          transform: translate3d(0, 0, 0);
        }
        `
            : ""
        }
      `;
      document.head.appendChild(style);
    }
  }
};

export enum ThemeAnimationType {
  CIRCLE = "circle",
  BLUR_CIRCLE = "blur-circle",
}

interface ReactThemeSwitchAnimationHook {
  ref: React.RefObject<HTMLButtonElement | null>;
  toggleSwitchTheme: (onChange?: () => void) => Promise<void>;
}

export interface ReactThemeSwitchAnimationProps {
  duration?: number;
  easing?: string;
  pseudoElement?: string;
  animationType?: ThemeAnimationType;
  styleId?: string;
}

export const useThemeAnimation = (
  props?: ReactThemeSwitchAnimationProps,
): ReactThemeSwitchAnimationHook => {
  const {
    duration: propsDuration = 750,
    easing = "ease-in-out",
    pseudoElement = "::view-transition-new(root)",
    animationType = ThemeAnimationType.CIRCLE,
    styleId = "theme-switch-style",
  } = props || {};

  const isHighResolution =
    typeof window !== "undefined" &&
    (window.innerWidth >= 3000 || window.innerHeight >= 2000);

  const duration = isHighResolution
    ? Math.max(propsDuration * 0.8, 500)
    : propsDuration;

  useEffect(() => {
    injectBaseStyles();
  }, []);

  const ref = useRef<HTMLButtonElement>(null);

  const toggleSwitchTheme = async (onChange?: () => void) => {
    if (
      !ref.current ||
      !document.startViewTransition ||
      window.matchMedia("(prefers-reduced-motion: reduce)").matches
    ) {
      onChange?.();
      return;
    }

    const existingStyle = document.getElementById(styleId);
    if (existingStyle) {
      existingStyle.remove();
    }

    const { top, left, width, height } = ref.current.getBoundingClientRect();
    const x = left + width / 2;
    const y = top + height / 2;

    // Calculate the distance to each corner of the viewport
    const topLeft = Math.hypot(x, y);
    const topRight = Math.hypot(window.innerWidth - x, y);
    const bottomLeft = Math.hypot(x, window.innerHeight - y);
    const bottomRight = Math.hypot(
      window.innerWidth - x,
      window.innerHeight - y,
    );

    // Find the maximum distance to ensure animation covers the entire viewport
    const maxRadius = Math.max(topLeft, topRight, bottomLeft, bottomRight);

    await document.startViewTransition(() => {
      flushSync(() => {
        onChange?.();
      });
    }).ready;

    if (animationType === ThemeAnimationType.CIRCLE) {
      document.documentElement.animate(
        {
          clipPath: [
            `circle(0px at ${x}px ${y}px)`,
            `circle(${maxRadius}px at ${x}px ${y}px)`,
          ],
        },
        {
          duration,
          easing,
          pseudoElement,
        },
      );
    }
  };

  return {
    ref,
    toggleSwitchTheme,
  };
};
