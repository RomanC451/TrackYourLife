import { PropsWithChildren } from "react";
import { Minus, Plus } from "lucide-react";
import { useLocalStorage } from "usehooks-ts";

import { queryClient } from "@/queryClient";

import { ModeToggle } from "../mode-toggle";
import { Button } from "../ui/button";
import { Input } from "../ui/input";

function RootNavbar({
  children,
  themeToggle,
}: PropsWithChildren<{ themeToggle: boolean }>) {
  const [throttling, setThrottling] = useLocalStorage("throttling", 5);

  return (
    <nav className="m-4 sm:px-8">
      <ul className="flex justify-between gap-10">
        <div className="flex w-full flex-wrap justify-between">
          {children}
          {import.meta.env.MODE === "development" && (
            <>
              <li>
                <Button
                  variant="outline"
                  onClick={() => {
                    queryClient.clear();
                  }}
                >
                  Rem queries
                </Button>
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
