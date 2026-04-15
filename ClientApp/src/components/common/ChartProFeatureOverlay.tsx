import { Crown } from "lucide-react";
import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

type ChartProFeatureOverlayProps = {
  /** When true, covers the chart area with the Pro upsell (e.g. `!isPro`). */
  show: boolean;
  title: string;
  description: string;
  /** Use when the overlay sits directly under a card header (no extra top border line). */
  omitTopBorder?: boolean;
  className?: string;
};

/** Blurred overlay for chart regions that require a Pro plan. Use inside a `relative` parent, like ChartLoadingOverlay. */
export function ChartProFeatureOverlay({
  show,
  title,
  description,
  omitTopBorder = false,
  className,
}: ChartProFeatureOverlayProps) {
  if (!show) return null;

  return (
    <div
      className={cn(
        "pointer-events-auto absolute inset-0 z-10 flex flex-col items-center justify-center gap-4 border border-border bg-background/65 p-6 text-center shadow-sm backdrop-blur-[3px]",
        omitTopBorder && "border-t-0",
        className,
      )}
    >
      <div className="rounded-full bg-primary/10 p-3">
        <Crown className="h-8 w-8 text-primary" aria-hidden />
      </div>
      <div className="max-w-sm space-y-2">
        <p className="text-base font-semibold text-foreground">{title}</p>
        <p className="text-sm text-muted-foreground">{description}</p>
      </div>
      <Button asChild>
        <Link to="/upgrade">Upgrade to Pro</Link>
      </Button>
    </div>
  );
}
