import { Info } from "lucide-react";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  HybridTooltip,
  HybridTooltipContent,
  HybridTooltipTrigger,
  TouchProvider,
} from "@/components/ui/hybrid-tooltip";
import { cn } from "@/lib/utils";
import type { PerformanceCalculationMethod } from "../queries/useExercisePerformanceQuery";

interface CalculationMethodSelectorProps {
  value: PerformanceCalculationMethod;
  onValueChange: (value: PerformanceCalculationMethod) => void;
  /** When true, the select is disabled (e.g. when query is fetching). */
  disabled?: boolean;
  /** When true, the select is disabled (e.g. when query is fetching). */
  loading?: boolean;
}

export function CalculationMethodSelector({
  value,
  onValueChange,
  disabled = false,
  loading = false,
}: CalculationMethodSelectorProps) {
  const isDisabled = disabled || loading;

  return (
    <div className={cn("flex items-center gap-2 justify-center lg:justify-end")}>
      <Label htmlFor="calculation-method" className="text-sm">
        Calculation Method:
      </Label>
      <TouchProvider>
        <HybridTooltip>
          <HybridTooltipTrigger asChild>
            <Info className="h-4 w-4 cursor-help text-muted-foreground" />
          </HybridTooltipTrigger>
          <HybridTooltipContent className="max-w-xs">
            <div className="space-y-2">
              <div>
                <strong>Sequential:</strong> Compares each workout to the previous one and averages all improvements (e.g., workout 2 vs 1, workout 3 vs 2, etc.).
              </div>
              <div>
                <strong>First vs Last:</strong> Compares the first workout to the most recent one in the date range.
              </div>
            </div>
          </HybridTooltipContent>
        </HybridTooltip>
      </TouchProvider>
      <Select
        value={value}
        onValueChange={(value) =>
          onValueChange(value as PerformanceCalculationMethod)
        }
        disabled={isDisabled}
      >
        <SelectTrigger id="calculation-method" className={cn("w-[200px]")}>
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="Sequential">
            Average Improvement
          </SelectItem>
          <SelectItem value="FirstVsLast">
            First vs Last
          </SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}
