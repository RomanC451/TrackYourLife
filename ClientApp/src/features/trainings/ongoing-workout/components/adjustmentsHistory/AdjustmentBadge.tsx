type AdjustmentBadgeProps = {
  value: number;
  unit: string;
};

function AdjustmentBadge({ value, unit }: AdjustmentBadgeProps) {
  const isPositive = value > 0;
  const isZero = value === 0;
  const color = isZero
    ? "bg-gray-400"
    : isPositive
      ? "bg-green-600"
      : "bg-red-600";
  const sign = isPositive ? "+" : "";
  return (
    <span
      className={`rounded-full px-2 py-1 text-xs font-semibold text-white ${color} mr-2 text-nowrap`}
    >
      {sign}
      {value} {unit}
    </span>
  );
}

export default AdjustmentBadge;
