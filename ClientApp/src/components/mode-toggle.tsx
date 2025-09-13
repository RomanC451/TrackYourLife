import { Moon, Sun } from "lucide-react";

import { useTheme } from "@/components/theme-provider";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { useThemeAnimation } from "./theme-animation";

export function ModeToggle() {
  const { theme, setTheme } = useTheme();

  const { toggleSwitchTheme, ref } = useThemeAnimation();

  const handleLight = () => {
    if (theme === "light") {
      return;
    }

    toggleSwitchTheme(() => setTheme("light"));
  };

  const handleDark = () => {
    if (theme === "dark") {
      return;
    }

    toggleSwitchTheme(() => setTheme("dark"));
  };

  const handleSystem = () => {
    const systemTheme = window.matchMedia("(prefers-color-scheme: dark)")
      .matches
      ? "dark"
      : "light";

    if (theme === "system" || theme === systemTheme) {
      return;
    }

    toggleSwitchTheme(() => setTheme("system"));
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button ref={ref} variant="outline" size="icon">
          <Sun className="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
          <Moon className="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
          <span className="sr-only">Toggle theme</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem onClick={handleLight}>Light</DropdownMenuItem>
        <DropdownMenuItem onClick={handleDark}>Dark</DropdownMenuItem>
        <DropdownMenuItem onClick={handleSystem}>System</DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
