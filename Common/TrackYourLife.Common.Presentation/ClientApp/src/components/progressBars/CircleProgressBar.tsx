import { ArcElement, Chart as ChartJS, Legend, Tooltip } from "chart.js";
import React from "react";
import { Doughnut } from "react-chartjs-2";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";
import { cn } from "../../utils/utils";

ChartJS.register(ArcElement, Tooltip, Legend);

const doughnutGraphOptions = {
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
  completionPercentage: completitionPercentage,
}) => {
  if (completitionPercentage < 0) {
    completitionPercentage = 0;
  }
  if (completitionPercentage > 100) {
    completitionPercentage = 100;
  }

  const percentageFontStyle =
    "text-gray font-[Nunito_Sans] text-[14px] font-semibold leading-[15.4px] absolute bottom-[-20px]";

  const doughnutGraphData = {
    labels: [""],
    datasets: [
      {
        label: "Calories Graph",
        data: [completitionPercentage, 100 - completitionPercentage],
        backgroundColor: [color, "transparent"],
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

      <p className={cn(percentageFontStyle, "left-[10px]")}>0%</p>
      <p className={cn(percentageFontStyle, "right-[-1px]")}>100%</p>
    </div>
  );
};

export default CircleProgressBar;
