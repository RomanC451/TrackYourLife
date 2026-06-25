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
import type { ReadingOverviewType } from "@/services/openapi";

type ReadingOverviewTypeDropDownMenuProps = {
  overviewType: ReadingOverviewType;
  setOverviewType: React.Dispatch<React.SetStateAction<ReadingOverviewType>>;
  loading?: boolean;
};

function ReadingOverviewTypeDropDownMenu({
  overviewType,
  setOverviewType,
  loading = false,
}: ReadingOverviewTypeDropDownMenuProps) {
  const [open, setOpen] = useState(false);

  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          disabled={loading}
          className={cn("w-[112px]")}
        >
          {capitalizeFirstLetter(overviewType)}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-[200px]">
        <DropdownMenuItem onSelect={() => setOverviewType("Daily")}>
          Daily
        </DropdownMenuItem>
        <DropdownMenuItem onSelect={() => setOverviewType("Weekly")}>
          Weekly
        </DropdownMenuItem>
        <DropdownMenuItem onSelect={() => setOverviewType("Monthly")}>
          Monthly
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default ReadingOverviewTypeDropDownMenu;
