import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Minus, Plus } from "lucide-react";
import * as React from "react";
import { useEffect, useState } from "react";
import { Bar, BarChart, ResponsiveContainer } from "recharts";
import { toast } from "sonner";

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
import { toastDefaultServerError } from "~/data/apiSettings";
import useUserCaloriesGoalQuery from "~/features/health/hooks/useUserCaloriesGoalQuery";
import {
  UserGoalApi,
  UserGoalPerPeriod,
  UserGoalResponse,
  UserGoalType,
} from "~/services/openapi";
import { getDateOnly } from "~/utils/date";

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

const GOAL_STEP = 50;

const userGoalApi = new UserGoalApi();

export const SetCaloriesGoalDrawer = () => {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [value, setValue] = React.useState(2000);

  function onClick(adjustment: number) {
    setValue(value + adjustment);
  }

  const queryClient = useQueryClient();

  const { userCaloriesGoalQuery } = useUserCaloriesGoalQuery();

  useEffect(() => {
    if (!userCaloriesGoalQuery.data) return;
    setValue(userCaloriesGoalQuery.data.value);
  }, [userCaloriesGoalQuery.data]);

  const setCaloriesGoalMutation = useMutation({
    mutationFn: () =>
      userGoalApi
        .addGoal({
          type: UserGoalType.Calories,
          value: value,
          perPeriod: UserGoalPerPeriod.Day,
          startDate: getDateOnly(),
          force: true,
        })
        .then((res) => res.data),
    onSuccess: () => {
      setDrawerOpen(false);
      toast.success("Calories goal has been set.");

      queryClient.invalidateQueries({
        queryKey: ["userCaloriesGoal"],
        exact: true,
      });

      queryClient.setQueryData(
        ["userCaloriesGoal"],
        (oldData: UserGoalResponse) => ({
          goal: { ...oldData, value },
        }),
      );
      console.log("success");
    },
    onError: toastDefaultServerError,
  });

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
                onClick={() => onClick(-GOAL_STEP)}
                // disabled={goal <= 200}
              >
                <Minus className="h-4 w-4" />
                <span className="sr-only">Decrease</span>
              </Button>
              <div
                className="flex-1 text-center"
                onWheel={(event) => {
                  if (event.deltaY < 0) {
                    onClick(GOAL_STEP); // Increase the goal when scrolling up
                  } else {
                    onClick(-GOAL_STEP); // Decrease the goal when scrolling down
                  }
                }}
              >
                <Input
                  className="h-20 border-0 text-center text-7xl font-bold tracking-tighter outline-0 focus-visible:ring-0 focus-visible:ring-offset-0"
                  value={value}
                  inputMode="numeric"
                  onChange={(e) => {
                    e.target.value = e.target.value
                      .replace(/[^\d.]/g, "")
                      .replace(/(\..*)\./g, "$1");
                    // if (e.target.value === "") return;
                    setValue(parseFloat(e.target.value || "0"));
                  }}
                ></Input>
                <div className="text-[0.70rem] uppercase text-muted-foreground">
                  Calories/day
                </div>
              </div>
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8 shrink-0 rounded-full"
                onClick={() => onClick(GOAL_STEP)}
                // disabled={goal >= 400}
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
            <Button onClick={() => setCaloriesGoalMutation.mutate()}>
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
