import { ArcElement, Chart as ChartJS, Legend, Tooltip } from "chart.js";
import clsx from "clsx";
import React from "react";
import { Doughnut } from "react-chartjs-2";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";

ChartJS.register(ArcElement, Tooltip, Legend);

const doughnutGraphOptions = {
  cutout: "80%",
  plugins: {
    legend: {
      display: false
    }
  },
  responsive: true,
  maintainAspectRatio: false,
  circumference: 180,
  rotation: -180 / 2,
  radius: 140
};

interface Props {
  color: string;
  darkColor: string;
  completitionPercentage: number;
}

const CircleProgressBar: React.FC<Props> = ({
  color,
  darkColor,
  completitionPercentage
}) => {
  if (0 > completitionPercentage || completitionPercentage > 100) {
    throw new Error("completitionPercentage must be between 0 and 100");
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
        borderColor: "transparent"
      }
    ]
  };

  const backgorundDoughnutGraphData = {
    labels: [""],
    datasets: [
      {
        ...doughnutGraphData.datasets[0],
        data: [300],
        backgroundColor: [darkColor]
      }
    ]
  };

  return (
    <div className="relative w-[291px] h-[145px]">
      <AbsoluteCenterChildrenLayout>
        <Doughnut
          data={backgorundDoughnutGraphData}
          options={doughnutGraphOptions}
        />
      </AbsoluteCenterChildrenLayout>
      <AbsoluteCenterChildrenLayout>
        <Doughnut data={doughnutGraphData} options={doughnutGraphOptions} />
      </AbsoluteCenterChildrenLayout>

      <p className={clsx(percentageFontStyle, "left-[10px]")}>0%</p>
      <p className={clsx(percentageFontStyle, "right-[-1px]")}>100%</p>
    </div>
  );
};

export default CircleProgressBar;
