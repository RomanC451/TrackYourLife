import { useEffect, useRef } from "react";

export function usePerformanceMonitor(componentName: string, enabled = true) {
  const renderStartTime = useRef<number>(0);
  const renderCount = useRef<number>(0);

  useEffect(() => {
    if (!enabled) return;

    renderStartTime.current = performance.now();
    renderCount.current += 1;

    return () => {
      const renderTime = performance.now() - renderStartTime.current;

      // Log slow renders in development
      if (process.env.NODE_ENV === "development" && renderTime > 16) {
        console.warn(
          `🐌 Slow render detected in ${componentName}: ${renderTime.toFixed(2)}ms`,
        );
      }

      // Log memory usage if available
      if ("memory" in performance) {
        const memory = (performance as any).memory;
        const memoryUsage = memory.usedJSHeapSize / 1024 / 1024; // MB

        if (memoryUsage > 50) {
          // Warn if memory usage is high
          console.warn(
            `🧠 High memory usage in ${componentName}: ${memoryUsage.toFixed(2)}MB`,
          );
        }
      }
    };
  });

  return {
    renderCount: renderCount.current,
    isSlowRender: (threshold = 16) => {
      const renderTime = performance.now() - renderStartTime.current;
      return renderTime > threshold;
    },
  };
}
