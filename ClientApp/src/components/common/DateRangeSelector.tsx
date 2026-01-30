import { format, startOfDay, subDays, subMonths, subYears } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { DateRange } from "react-day-picker";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { screensEnum } from "@/constants/tailwindSizes";
import { useAppGeneralStateContext } from "@/contexts/AppGeneralContextProvider";
import { cn } from "@/lib/utils";
import { Separator } from "@radix-ui/react-separator";

const PRESETS: { label: string; getRange: () => DateRange | undefined }[] = [
  { label: "Last week", getRange: () => ({ from: startOfDay(subDays(new Date(), 7)), to: new Date() }) },
  { label: "Last month", getRange: () => ({ from: startOfDay(subMonths(new Date(), 1)), to: new Date() }) },
  { label: "Last year", getRange: () => ({ from: startOfDay(subYears(new Date(), 1)), to: new Date() }) },
  { label: "All time", getRange: () => undefined },
];

type DateRangeSelectorProps = {
  selectedRange?: DateRange;
  handleRangeSelect: (range: DateRange | undefined) => void;
  disabled?: boolean;
  loading?: boolean;
};

function formatDateRange(selectedRange?: DateRange) {
  if (!selectedRange?.from) {
    return <span>All time</span>;
  }

  if (selectedRange.to) {
    return (
      <>
        {format(selectedRange.from, "LLL dd, y")} -{" "}
        {format(selectedRange.to, "LLL dd, y")}
      </>
    );
  }

  return format(selectedRange.from, "LLL dd, y");
}

export function DateRangeSelector({
  selectedRange,
  handleRangeSelect,
  disabled = false,
  loading = false,
}: DateRangeSelectorProps) {
  const { screenSize } = useAppGeneralStateContext();
  const isDisabled = disabled || loading;

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          disabled={isDisabled}
          className={cn(
            "w-full max-w-[280px] justify-center text-left font-normal",
            !selectedRange && "text-muted-foreground",
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {formatDateRange(selectedRange)}
        </Button>
      </PopoverTrigger>
      <PopoverContent
        className={cn(
          "w-auto p-0!"
        )}
        align="start"
      >
        <div className="flex flex-wrap gap-1 border-b px-2 py-1.5 justify-around">
          {PRESETS.map(({ label, getRange }, index) => (
            <>
              <Button
                key={label}
                variant="ghost"
                size="sm"
                className="h-8 text-xs"
                onClick={() => handleRangeSelect(getRange())}
              >
                {label}
              </Button>
              {index < PRESETS.length - 1 && <Separator orientation="vertical" className="w-px bg-border" />}
            </>
          ))}
        </div>
        <Calendar
          className="p-2"
          classNames={{
            months: cn(
              "flex flex-col sm:flex-row space-y-4 sm:space-x-4 sm:space-y-0",
              "items-center justify-center sm:items-stretch sm:justify-start",
            ),
          }}
          initialFocus
          mode="range"
          defaultMonth={selectedRange?.from}
          selected={selectedRange}
          onSelect={handleRangeSelect}
          numberOfMonths={screenSize.width >= screensEnum.sm ? 2 : 1}
        />
      </PopoverContent>
    </Popover>
  );
}
