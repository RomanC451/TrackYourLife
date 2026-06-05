import { cn } from "@/lib/utils";

type AdjustmentBadgeProps = {
  value: number;
  unit: string;
};

function AdjustmentBadge({ value, unit }: AdjustmentBadgeProps) {
  const isPositive = value > 0;
  const isZero = value === 0;
  const sign = isPositive ? "+" : "";

  return (
    <span
      className={cn(
        "inline-flex rounded-full px-2 py-1 text-xs font-semibold text-white",
        isZero && "bg-muted-foreground",
        isPositive && "bg-green-600",
        !isPositive && !isZero && "bg-red-600",
      )}
    >
      {sign}
      {value.toFixed(2)} {unit}
    </span>
  );
}

export default AdjustmentBadge;
