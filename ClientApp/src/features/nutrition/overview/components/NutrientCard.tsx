import { memo, useEffect } from "react";
import { motion, useAnimation } from "framer-motion";
import { Cell, Pie, PieChart } from "recharts";

import { useTheme } from "@/components/theme-provider";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { OverviewType } from "./NutrientsCharts";

interface NutrientCardProps {
  title: string;
  current: number;
  target: number;
  unit: string;
  color: string;
  overviewType: OverviewType;
  isLoading: boolean;
}

const NutrientCardComponent = ({
  title,
  current,
  target,
  unit,
  color,
  overviewType,
  isLoading,
}: NutrientCardProps) => {
  const percentage = target === 0 ? 0 : Math.round((current / target) * 100);
  const remaining = target - current;

  const { theme } = useTheme();

  let data;
  if (isLoading) {
    data = [
      { name: "Current", value: 10 },
      { name: "Remaining", value: 90 },
    ];
  } else {
    data = [
      { name: "Current", value: current },
      { name: "Remaining", value: remaining > 0 ? remaining : 0 },
    ];
  }

  const COLORS = [color, "hsl(var(--muted))"];

  const controls = useAnimation();

  useEffect(() => {
    const animate = async () => {
      if (isLoading) {
        await controls.start({
          rotate: [0, 360],
          transition: { duration: 2, ease: "linear", repeat: Infinity },
        });
      } else {
        await controls.start({
          rotate: 360,
          transition: { duration: 1, ease: "linear" },
        });
      }
    };

    animate();
  }, [isLoading, controls]);

  return (
    <Card className="min-w-[200px] overflow-hidden">
      <CardHeader className="pb-2">
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <motion.div
          animate={controls}
          className="grid h-[200px] w-full place-content-center"
        >
          <PieChart width={200} height={200}>
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              innerRadius={60}
              outerRadius={80}
              fill="#8884d8"
              paddingAngle={5}
              dataKey="value"
              stroke={theme === "dark" ? "white" : "black"}
            >
              {data.map((_, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={COLORS[index % COLORS.length]}
                />
              ))}
            </Pie>
          </PieChart>
        </motion.div>
        <div className="mt-4 p-4 text-center">
          <p className="text-2xl font-bold">
            {current.toFixed()} / {target.toFixed()} {unit}
          </p>
          <p className="text-sm text-muted-foreground">
            {percentage}% of{" "}
            {overviewType === "day"
              ? "daily"
              : overviewType === "week"
                ? "weekly"
                : "monthly"}
            target
          </p>
        </div>
      </CardContent>
    </Card>
  );
};

export const NutrientCard = memo(NutrientCardComponent);

NutrientCard.displayName = "NutrientCard";
