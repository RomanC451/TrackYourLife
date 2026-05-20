import * as ScrollAreaPrimitive from "@radix-ui/react-scroll-area";

import { ScrollBar } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import type { YoutubeCategorySettingsDto } from "@/services/openapi";
import { cn } from "@/lib/utils";

import type { YoutubeCategoryListFilter } from "../../queries/youtubeQueries";

export type CategoryTabValue = YoutubeCategoryListFilter;

interface CategoryTabsProps {
  categories: YoutubeCategorySettingsDto[];
  value: CategoryTabValue;
  onValueChange: (value: CategoryTabValue) => void;
  isLoading?: boolean;
}

const tagButtonClass = (isActive: boolean) =>
  cn(
    "inline-flex shrink-0 items-center gap-2 rounded-lg border px-3 py-2 text-sm font-medium transition-all",
    isActive
      ? "border-primary bg-primary/10 text-primary"
      : "border-border bg-background hover:border-primary/50 hover:bg-secondary",
  );

function CategoryTabs({
  categories,
  value,
  onValueChange,
  isLoading,
}: CategoryTabsProps) {
  if (isLoading) {
    return (
      <div className="flex gap-2 pb-3">
        <Skeleton className="h-10 w-14 shrink-0 rounded-lg" />
        <Skeleton className="h-10 w-24 shrink-0 rounded-lg" />
        <Skeleton className="h-10 w-20 shrink-0 rounded-lg" />
      </div>
    );
  }

  return (
    <ScrollAreaPrimitive.Root
      type="auto"
      className="relative w-full min-w-0 max-w-full overflow-hidden"
    >
      <ScrollAreaPrimitive.Viewport className="w-full max-w-full min-w-0 rounded-[inherit] [&>div]:block!">
        <div className="flex w-max gap-2 pb-3">
          <button
            type="button"
            onClick={() => onValueChange("all")}
            className={tagButtonClass(value === "all")}
          >
            All
          </button>
          {categories.map((c) => {
            const isActive = value === c.id;
            return (
              <button
                key={c.id}
                type="button"
                onClick={() => onValueChange(c.id)}
                className={tagButtonClass(isActive)}
              >
                {c.name}
              </button>
            );
          })}
        </div>
      </ScrollAreaPrimitive.Viewport>
      <ScrollBar orientation="horizontal" className="pt-1.5" />
      <ScrollAreaPrimitive.Corner />
    </ScrollAreaPrimitive.Root>
  );
}

export default CategoryTabs;
