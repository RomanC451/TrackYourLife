import type { LucideIcon } from "lucide-react";
import { Dumbbell } from "lucide-react";

import { cn } from "@/lib/utils";

/** Lowercase keys matching `public/muscleGroupsIcons/<key>.svg` */
const MUSCLE_GROUP_ICON_SLUGS = [
  "arms",
  "back",
  "chest",
  "core",
  "legs",
] as const;

function iconBaseUrl(): string {
  const base = import.meta.env.BASE_URL;
  return base.endsWith("/") ? base : `${base}/`;
}

function muscleGroupIconSrc(slug: (typeof MUSCLE_GROUP_ICON_SLUGS)[number]): string {
  return `${iconBaseUrl()}muscleGroupsIcons/${slug}.svg`;
}

/**
 * Returns the public SVG URL when `muscleGroup` (lowercased) contains one of {@link MUSCLE_GROUP_ICON_SLUGS},
 * otherwise Lucide {@link Dumbbell}.
 */
function getMuscleGroupWorkoutIcon(muscleGroup: string): string | LucideIcon {
  const n = muscleGroup.trim().toLowerCase();
  const slug = MUSCLE_GROUP_ICON_SLUGS.find((key) => n.includes(key));
  return slug ? muscleGroupIconSrc(slug) : Dumbbell;
}

export type MuscleGroupWorkoutIconProps = {
  muscleGroups: readonly string[] | undefined | null;
  className?: string;
};

function isAssetIcon(icon: string | LucideIcon): icon is string {
  return typeof icon === "string";
}

/**
 * Renders a monochrome SVG from `public/muscleGroupsIcons/` tinted with `currentColor` (use `text-*` classes).
 */
export function MuscleGroupWorkoutIcon({
  muscleGroups,
  className,
}: MuscleGroupWorkoutIconProps) {
  if (!muscleGroups?.length) {
    return <Dumbbell className={className} />;
  }


  for (const label of muscleGroups) {
    const icon = getMuscleGroupWorkoutIcon(label);
    if (isAssetIcon(icon)) {
      return (
        <span
          aria-hidden
          className={cn("inline-block shrink-0 bg-current", className)}
          style={{
            maskImage: `url("${icon}")`,
            WebkitMaskImage: `url("${icon}")`,
            maskSize: "contain",
            maskRepeat: "no-repeat",
            maskPosition: "center",
          }}
        />
      );
    }
  }

  return <Dumbbell className={className} />;
}
