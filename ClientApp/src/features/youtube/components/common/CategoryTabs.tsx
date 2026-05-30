import * as ScrollAreaPrimitive from "@radix-ui/react-scroll-area";
import { Star } from "lucide-react";

import { ScrollBar } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import type { YoutubeCategorySettingsDto } from "@/services/openapi";
import { cn } from "@/lib/utils";

import {
  type YoutubeCategoryListFilter,
  youtubeCategoryListFilterFavorites,
} from "../../queries/youtubeQueries";

export type CategoryTabValue = YoutubeCategoryListFilter;

interface CategoryTabsProps {
  categories: YoutubeCategorySettingsDto[];
  value: CategoryTabValue;
  onValueChange: (value: CategoryTabValue) => void;
  isLoading?: boolean;
  showFavoritesTab?: boolean;
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
  showFavoritesTab = false,
}: CategoryTabsProps) {
  if (isLoading) {
    return (
      <div className="flex items-center gap-2 pb-3">
        <Skeleton className="h-10 w-28 shrink-0 rounded-lg" />
        <Skeleton className="h-10 w-14 shrink-0 rounded-lg" />
        <div className="ml-4 flex gap-2 border-l border-border pl-6">
          <Skeleton className="h-10 w-24 shrink-0 rounded-lg" />
          <Skeleton className="h-10 w-20 shrink-0 rounded-lg" />
        </div>
      </div>
    );
  }

  return (
    <ScrollAreaPrimitive.Root
      type="auto"
      className="relative w-full min-w-0 max-w-full overflow-hidden"
    >
      <ScrollAreaPrimitive.Viewport className="w-full max-w-full min-w-0 rounded-[inherit] [&>div]:block!">
        <div className="flex w-max items-center gap-2 pb-3">
          {showFavoritesTab ? (
            <button
              type="button"
              onClick={() => onValueChange(youtubeCategoryListFilterFavorites)}
              className={tagButtonClass(value === youtubeCategoryListFilterFavorites)}
            >
              <Star className="h-3.5 w-3.5 fill-current" />
              Favorites
            </button>
          ) : null}
          <button
            type="button"
            onClick={() => onValueChange("all")}
            className={tagButtonClass(value === "all")}
          >
            All
          </button>
          {categories.length > 0 ? (
            <div className="ml-4 flex items-center gap-2 border-l border-border pl-6">
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
          ) : null}
        </div>
      </ScrollAreaPrimitive.Viewport>
      <ScrollBar orientation="horizontal" className="pt-1.5" />
      <ScrollAreaPrimitive.Corner />
    </ScrollAreaPrimitive.Root>
  );
}

export default CategoryTabs;
