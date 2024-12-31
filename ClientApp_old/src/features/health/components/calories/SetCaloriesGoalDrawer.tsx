import { Minus, Plus } from "lucide-react";
import * as React from "react";
import { useEffect, useState } from "react";
import { Bar, BarChart, ResponsiveContainer } from "recharts";

import { Button } from "~/chadcn/ui/button";
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerDescription,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
} from "~/chadcn/ui/drawer";
import { Input } from "~/chadcn/ui/input";
import { InputError } from "~/chadcn/ui/input-error";
import useSetCaloriesGoalMutation from "~/features/health/mutations/useSetCaloriesGoalMutation";
import useCaloriesGoalQuery from "~/features/health/queries/useCaloriesGoalQuery";
import { cn } from "~/utils";

const data = [
  {
    goal: 400,
  },
  {
    goal: 300,
  },
  {
    goal: 200,
  },
  {
    goal: 300,
  },
  {
    goal: 200,
  },
  {
    goal: 278,
  },
  {
    goal: 189,
  },
  {
    goal: 239,
  },
  {
    goal: 300,
  },
  {
    goal: 200,
  },
  {
    goal: 278,
  },
  {
    goal: 189,
  },
  {
    goal: 349,
  },
];

const GOAL_STEP = { button: 50, scroll: 10 };

export const SetCaloriesGoalDrawer = () => {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const { caloriesGoalQuery, isPending: queryIsPending } =
    useCaloriesGoalQuery();

  const [value, setValue] = useState<number>(0);

  useEffect(() => {
    if (queryIsPending.isLoaded && caloriesGoalQuery.data) {
      setValue(caloriesGoalQuery.data.value);
    }
  }, [queryIsPending]);

  const {
    setCaloriesGoalMutation,
    isPending: mutationIsPending,
    error: mutationError,
  } = useSetCaloriesGoalMutation();

  function adjustValue(adjustment: number) {
    if (value !== null) {
      setValue(Math.max(0, value + adjustment));
    }
  }

  return (
    <Drawer open={drawerOpen} onOpenChange={setDrawerOpen}>
      <DrawerTrigger asChild>
        <Button variant="outline" className="rounded-full">
          Set goal
        </Button>
      </DrawerTrigger>
      <DrawerContent>
        <div className="mx-auto w-full max-w-sm">
          <DrawerHeader>
            <DrawerTitle>Calories Goal</DrawerTitle>
            <DrawerDescription>Set your daily calories goal.</DrawerDescription>
          </DrawerHeader>
          <div className="p-4 pb-0">
            <div className="flex items-center justify-center space-x-2">
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8 shrink-0 rounded-full"
                onClick={() => adjustValue(-GOAL_STEP.button)}
                disabled={
                  !queryIsPending.isLoaded || !mutationIsPending.isLoaded
                }
              >
                <Minus className="h-4 w-4" />
                <span className="sr-only">Decrease</span>
              </Button>
              <div
                className="flex-1 text-center"
                onWheel={(event) => {
                  if (!queryIsPending.isLoaded || !mutationIsPending.isLoaded)
                    return;
                  if (event.deltaY < 0) {
                    adjustValue(GOAL_STEP.scroll);
                  } else {
                    adjustValue(-GOAL_STEP.scroll);
                  }
                }}
              >
                <InputError
                  isError={mutationError !== undefined}
                  message={mutationError ?? ""}
                />
                <Input
                  className={
                    cn(
                      "h-20 border-0 text-center text-7xl font-bold tracking-tighter outline-0 focus-visible:ring-0 focus-visible:ring-offset-0",
                      setCaloriesGoalMutation.isError && "text-destructive",
                    ) as string
                  }
                  value={value}
                  inputMode="numeric"
                  onChange={(e) => {
                    e.target.value = e.target.value
                      .replace(/[^\d.]/g, "")
                      .replace(/(\..*)\./g, "$1");

                    setValue(parseFloat(e.target.value || "0"));
                  }}
                />

                <div className="text-[0.70rem] uppercase text-muted-foreground">
                  Calories/day
                </div>
              </div>
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8 shrink-0 rounded-full"
                onClick={() => adjustValue(GOAL_STEP.button)}
                disabled={
                  !queryIsPending.isLoaded || !mutationIsPending.isLoaded
                }
              >
                <Plus className="h-4 w-4" />
                <span className="sr-only">Increase</span>
              </Button>
            </div>
            <div className="mt-3 h-[120px]">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={data}>
                  <Bar
                    dataKey="goal"
                    style={
                      {
                        fill: "hsl(var(--foreground))",
                        opacity: 0.9,
                      } as React.CSSProperties
                    }
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
          <DrawerFooter>
            <Button
              onClick={() => {
                setCaloriesGoalMutation.mutateAsync(
                  { value },
                  {
                    onSuccess: () => setDrawerOpen(false),
                  },
                );
              }}
              disabled={!queryIsPending.isLoaded || !mutationIsPending.isLoaded}
            >
              Submit
            </Button>
            <DrawerClose asChild>
              <Button variant="outline">Cancel</Button>
            </DrawerClose>
          </DrawerFooter>
        </div>
      </DrawerContent>
    </Drawer>
  );
};
