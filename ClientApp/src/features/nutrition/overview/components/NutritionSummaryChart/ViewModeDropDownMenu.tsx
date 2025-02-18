import { useState } from "react";
import { ChevronsUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { viewModeConfig } from "../../data/consts";
import { ViewMode } from "../../data/types";

type ViewModeDropDownMenuProps = {
  viewMode: ViewMode;
  setViewMode: React.Dispatch<React.SetStateAction<ViewMode>>;
};

function ViewModeDropDownMenu({
  viewMode,
  setViewMode,
}: ViewModeDropDownMenuProps) {
  const [open, setOpen] = useState(false);
  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className="w-full min-w-[190px] max-w-[280px] sm:w-auto"
        >
          {viewModeConfig[viewMode].label}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className="w-[200px]">
        {Object.entries(viewModeConfig).map(([value, config]) => (
          <DropdownMenuItem
            key={value}
            onSelect={() => setViewMode(value as ViewMode)}
          >
            {config.label}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

export default ViewModeDropDownMenu;
