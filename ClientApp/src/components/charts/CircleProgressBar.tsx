import React from "react";
import {
  ArcElement,
  ChartData,
  Chart as ChartJS,
  ChartOptions,
  DefaultDataPoint,
  Legend,
  Tooltip,
} from "chart.js";
import { Doughnut } from "react-chartjs-2";

import AbsoluteCenterChildrenLayout from "@/layouts/AbsoluteCenterChildrenLayout";
import { cn } from "@/lib/utils";

ChartJS.register(ArcElement, Tooltip, Legend);

const doughnutGraphOptions: ChartOptions<"doughnut"> = {
  animation: {
    duration: 1000,
    easing: "easeInOutQuart",
    animateRotate: true,
  },

  cutout: "80%",
  plugins: {
    legend: {
      display: false,
    },
  },

  responsive: true,
  maintainAspectRatio: false,
  circumference: 180,
  rotation: -180 / 2,
  radius: 140,
};

interface Props {
  color: string;
  darkColor: string;
  completionPercentage: number;
}

const CircleProgressBar: React.FC<Props> = ({
  color,
  darkColor,
  completionPercentage: completionPercentage,
}) => {
  const overCompletionPercentage = Math.min(
    Math.max(completionPercentage - 100, 0),
    100,
  );

  const percentageFontStyle =
    "text-gray font-[Nunito_Sans] text-[14px] font-semibold leading-[15.4px] absolute bottom-[-20px]";

  const doughnutGraphData: ChartData<
    "doughnut",
    DefaultDataPoint<"doughnut">
  > = {
    labels: [""],
    datasets: [
      {
        label: "Calories Graph",
        data: [completionPercentage, 100 - Math.min(completionPercentage, 100)],
        backgroundColor: [color, "transparent"],
        hoverOffset: 4,
        borderRadius: 50,
        borderWidth: 5,
        borderColor: "transparent",
      },
    ],
  };

  const overCompletionDoughnutGraphData: ChartData<
    "doughnut",
    DefaultDataPoint<"doughnut">
  > = {
    labels: [""],
    datasets: [
      {
        label: "Over Completion Calories Graph",
        data: [overCompletionPercentage, 100 - overCompletionPercentage],
        backgroundColor: ["#991b1b", "transparent"],
        hoverOffset: 4,
        borderRadius: 50,
        borderWidth: 5,
        borderColor: "transparent",
      },
    ],
  };

  const backgroundDoughnutGraphData = {
    labels: [""],
    datasets: [
      {
        ...doughnutGraphData.datasets[0],
        data: [300],
        backgroundColor: [darkColor],
      },
    ],
  };

  return (
    <div className="relative h-[145px] w-[291px]">
      <AbsoluteCenterChildrenLayout>
        <Doughnut
          data={backgroundDoughnutGraphData}
          options={doughnutGraphOptions}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout>
        <Doughnut data={doughnutGraphData} options={doughnutGraphOptions} />
      </AbsoluteCenterChildrenLayout>
      {overCompletionPercentage === 0 ? null : (
        <AbsoluteCenterChildrenLayout>
          <Doughnut
            data={overCompletionDoughnutGraphData}
            options={doughnutGraphOptions}
          />
        </AbsoluteCenterChildrenLayout>
      )}

      <p className={cn(percentageFontStyle, "left-[10px]")}>0%</p>
      <p className={cn(percentageFontStyle, "-right-px")}>100%</p>
    </div>
  );
};

export default CircleProgressBar;
