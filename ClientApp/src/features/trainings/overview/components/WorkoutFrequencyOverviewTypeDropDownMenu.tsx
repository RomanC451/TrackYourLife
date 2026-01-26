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
import { OverviewType } from "@/services/openapi";

type WorkoutFrequencyOverviewTypeDropDownMenuProps = {
  overviewType: OverviewType;
  setOverviewType: React.Dispatch<React.SetStateAction<OverviewType>>;
};

function WorkoutFrequencyOverviewTypeDropDownMenu({
  overviewType,
  setOverviewType,
}: WorkoutFrequencyOverviewTypeDropDownMenuProps) {
  const [open, setOpen] = useState(false);

  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className="w-[112px]"
        >
          {capitalizeFirstLetter(overviewType)}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-[200px]">
        <DropdownMenuItem
          key={"weekly"}
          onSelect={() => setOverviewType("Weekly")}
        >
          Weekly
        </DropdownMenuItem>
        <DropdownMenuItem
          key={"monthly"}
          onSelect={() => setOverviewType("Monthly")}
        >
          Monthly
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default WorkoutFrequencyOverviewTypeDropDownMenu;
