import { format } from "date-fns";
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

type DateRangeSelectorProps = {
  selectedRange?: DateRange;
  handleRangeSelect: (range: DateRange | undefined) => void;
};

export function DateRangeSelector({
  selectedRange,
  handleRangeSelect,
}: DateRangeSelectorProps) {
  const { screenSize } = useAppGeneralStateContext();

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            "w-full max-w-[280px] justify-center text-left font-normal",
            !selectedRange && "text-muted-foreground",
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {selectedRange?.from ? (
            selectedRange.to ? (
              <>
                {format(selectedRange.from, "LLL dd, y")} -{" "}
                {format(selectedRange.to, "LLL dd, y")}
              </>
            ) : (
              format(selectedRange.from, "LLL dd, y")
            )
          ) : (
            <span>Pick a date range</span>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
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
