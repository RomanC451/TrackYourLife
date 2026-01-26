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
import type { PerformanceCalculationMethod } from "../queries/useExercisePerformanceQuery";

interface CalculationMethodSelectorProps {
  value: PerformanceCalculationMethod;
  onValueChange: (value: PerformanceCalculationMethod) => void;
}

export function CalculationMethodSelector({
  value,
  onValueChange,
}: CalculationMethodSelectorProps) {
  return (
    <div className="flex items-center lg:justify-end gap-2 justify-center">
      <Label htmlFor="calculation-method" className="text-sm">
        Calculation Method:
      </Label>
      <TouchProvider>
        <HybridTooltip>
          <HybridTooltipTrigger asChild>
            <Info className="h-4 w-4 text-muted-foreground cursor-help" />
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
      >
        <SelectTrigger id="calculation-method" className="w-[200px]">
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
