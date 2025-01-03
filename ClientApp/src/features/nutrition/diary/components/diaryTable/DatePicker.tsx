import React from "react";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { DateOnly, parseDateOnly } from "@/lib/date";
import { cn } from "@/lib/utils";

type DatePickerProps = {
  dateOnly: DateOnly;
  setDate: (date: Date) => void;
};

const DatePicker: React.FC<DatePickerProps> = ({ dateOnly, setDate }) => {
  const date = parseDateOnly(dateOnly);

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant={"outline"}
          className={cn(
            "h-8 w-[200px] justify-start text-left font-normal",
            !date && "text-muted-foreground",
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {date ? format(date, "PPP") : <span>Pick a date</span>}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={date}
          onSelect={(date) => {
            if (!date) return;
            const userDate = new Date(
              date.getTime() - date.getTimezoneOffset() * 60000,
            );
            setDate(userDate);
          }}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
};

export default DatePicker;
