import React from "react";
import {
  CategoryScale,
  Chart as ChartJS,
  Filler,
  Legend,
  LinearScale,
  LineElement,
  PointElement,
  ScriptableContext,
  Title,
  Tooltip,
} from "chart.js";
import { Line } from "react-chartjs-2";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
);

interface IProps {
  userData: number[];
  color: string;
  bgColor: string;
}

const LineChart: React.FC<IProps> = ({
  userData,
  color,
  bgColor,
}): React.JSX.Element => {
  const graphData = {
    labels: userData.map(() => ""),
    datasets: [
      {
        label: "Dataset 1",
        data: userData,
        borderColor: color,
        borderWidth: 1.5,
        backgroundColor: (context: ScriptableContext<"line">) => {
          const ctx = context.chart.ctx;
          const gradient = ctx.createLinearGradient(0, 0, 0, 30);
          gradient.addColorStop(0, color);
          gradient.addColorStop(1, bgColor);
          return gradient;
        },
        fill: "start",
      },
    ],
  };

  const graphOptions = {
    maintainAspectRatio: false,
    responsive: true,
    scales: {
      x: {
        display: false,
      },
      y: {
        display: false,
        min: Math.min(...userData) - 1,
        max: Math.max(...userData) + 1,
      },
    },
    elements: {
      line: {
        tension: 0.5,
      },
      point: {
        radius: 0,
        hitRadius: 0,
        hoverRadius: 0,
      },
    },
    plugins: {
      legend: {
        display: false,
      },
      filler: {
        propagate: false,
      },
    },
    interaction: {
      intersect: true,
    },
  };

  return <Line data={graphData} options={graphOptions} />;
};

export default LineChart;
