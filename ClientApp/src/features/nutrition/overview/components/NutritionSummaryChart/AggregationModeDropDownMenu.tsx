import { ChevronsUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { AggregationMode } from "@/services/openapi";

type AggregationModeDropDownMenuProps = {
  aggregationMode: AggregationMode;
  setAggregationMode: React.Dispatch<React.SetStateAction<AggregationMode>>;
  hidden?: boolean;
};

function AggregationModeDropDownMenu({
  aggregationMode,
  setAggregationMode,
  hidden,
}: AggregationModeDropDownMenuProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="outline" disabled={hidden} className="w-[152px]">
          {aggregationMode === "Average" ? "Daily Average" : "Total Sum"}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuItem onSelect={() => setAggregationMode("Average")}>
          Daily Average
        </DropdownMenuItem>
        <DropdownMenuItem onSelect={() => setAggregationMode("Sum")}>
          Total Sum
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default AggregationModeDropDownMenu;
