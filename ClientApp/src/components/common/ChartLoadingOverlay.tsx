import { cn } from "@/lib/utils";
import Spinner from "@/components/ui/spinner";

type ChartLoadingOverlayProps = {
  /** When true, shows the overlay with a loading spinner (e.g. use isDelayedFetching from useCustomQuery). */
  show: boolean;
  className?: string;
};

export function ChartLoadingOverlay({
  show,
  className,
}: ChartLoadingOverlayProps) {
  if (!show) return null;
  return (
    <div
      className={cn(
        "absolute inset-0 z-10 flex items-center justify-center rounded-lg bg-background/80",
        className,
      )}
      aria-busy
      aria-live="polite"
    >
      <Spinner className="h-10 w-10" />
    </div>
  );
}
