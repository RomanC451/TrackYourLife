import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import React from "react";
import { Button } from "~/chadcn/ui/button";
import { Calendar } from "~/chadcn/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "~/chadcn/ui/popover";
import { cn } from "~/utils/utils";

type DatePickerProps = {
  date: Date;
  setDate: (date: Date) => void;
};

const DatePicker: React.FC<DatePickerProps> = ({ date, setDate }) => {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant={"outline"}
          className={cn(
            "w-[200px] justify-start text-left font-normal h-8",
            !date && "text-muted-foreground"
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
              date.getTime() - date.getTimezoneOffset() * 60000
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
