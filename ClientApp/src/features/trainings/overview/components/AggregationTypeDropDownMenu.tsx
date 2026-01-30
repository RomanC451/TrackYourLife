import { useState } from "react";
import { ChevronsUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { capitalizeFirstLetter } from "@/lib/stringUtils";
import { cn } from "@/lib/utils";
import { AggregationType } from "@/services/openapi";

type AggregationTypeDropDownMenuProps = {
  aggregationType: AggregationType;
  setAggregationType: React.Dispatch<React.SetStateAction<AggregationType>>;
  disabled?: boolean;
  loading?: boolean;
};

function AggregationTypeDropDownMenu({
  aggregationType,
  setAggregationType,
  disabled = false,
  loading = false,
}: AggregationTypeDropDownMenuProps) {
  const [open, setOpen] = useState(false);
  const isDisabled = disabled || loading;

  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          disabled={isDisabled}
          className={cn("w-[112px]")}
        >
          {capitalizeFirstLetter(aggregationType)}
          <ChevronsUpDown className={cn("ml-2 h-4 w-4 shrink-0 opacity-50")} />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className={cn("w-[200px]")}>
        <DropdownMenuItem
          key="sum"
          onSelect={() => setAggregationType("Sum")}
        >
          Sum
        </DropdownMenuItem>
        <DropdownMenuItem
          key="average"
          onSelect={() => setAggregationType("Average")}
        >
          Average
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default AggregationTypeDropDownMenu;
