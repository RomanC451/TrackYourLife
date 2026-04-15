import { PropsWithChildren, useCallback, useEffect } from "react";
import { ChevronDown, FlaskConical, Minus, Plus } from "lucide-react";
import { toast } from "sonner";
import { useLocalStorage } from "usehooks-ts";

import { queryClient } from "@/queryClient";
import { UsersApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { ModeToggle } from "../mode-toggle";
import { Button } from "../ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { Input } from "../ui/input";

const usersApi = new UsersApi();

function RootNavbar({
  children,
  themeToggle,
}: PropsWithChildren<{ themeToggle: boolean }>) {
  const [throttling, setThrottling] = useLocalStorage("throttling", 5);

  const togglePlanTypeForDevelopment = useCallback(async () => {
    try {
      await usersApi.togglePlanTypeForDevelopment();
      await queryClient.invalidateQueries({
        queryKey: ["userData"],
      });
      toast.success("Plan type toggled");
    } catch (error) {
      const apiError = error as ApiError;
      toast.error(
        apiError.response?.data?.detail ?? "Failed to toggle plan type",
      );
    }
  }, []);

  useEffect(() => {
    if (import.meta.env.MODE !== "development") return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (!e.shiftKey || e.key.toLowerCase() !== "p") return;
      if (e.ctrlKey || e.metaKey || e.altKey) return;

      const el = document.activeElement as HTMLElement | null;
      if (
        el?.closest(
          'input, textarea, select, [contenteditable="true"], [role="combobox"]',
        )
      ) {
        return;
      }

      e.preventDefault();
      void togglePlanTypeForDevelopment();
    };

    globalThis.addEventListener("keydown", handleKeyDown);
    return () => globalThis.removeEventListener("keydown", handleKeyDown);
  }, [togglePlanTypeForDevelopment]);

  return (
    <nav className="m-4 sm:px-8">
      <ul className="flex justify-between gap-10">
        <div className="flex w-full flex-wrap justify-between">
          {children}
          {import.meta.env.MODE === "development" && (
            <>
              <li>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="outline" className="gap-1.5">
                      <FlaskConical className="h-4 w-4" aria-hidden />
                      Dev
                      <ChevronDown className="h-4 w-4 opacity-60" aria-hidden />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem
                      onSelect={() => {
                        queryClient.clear();
                      }}
                    >
                      Rem queries
                    </DropdownMenuItem>
                    <DropdownMenuItem
                      className="justify-between gap-4"
                      onSelect={() => {
                        void togglePlanTypeForDevelopment();
                      }}
                    >
                      <span>Toggle plan type</span>
                      <span className="text-xs text-muted-foreground">⇧P</span>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </li>
              <li>
                <div className="inline-flex gap-2">
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => setThrottling((prev) => prev - 1)}
                  >
                    <Minus />
                  </Button>
                  <Input
                    name="throttling"
                    min={0}
                    type="number"
                    value={throttling}
                    onChange={(e) => setThrottling(Number(e.target.value))}
                    className="w-24"
                  />
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => setThrottling((prev) => prev + 1)}
                  >
                    <Plus />
                  </Button>
                </div>
              </li>
            </>
          )}

          {themeToggle ? (
            <li>
              <ModeToggle />
            </li>
          ) : null}
        </div>
      </ul>
    </nav>
  );
}

export default RootNavbar;
