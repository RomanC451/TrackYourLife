import { ScriptableContext } from "chart.js";
import { tailwindColors } from "~/constants/tailwindColors";

export function getGraphConfig(userData: Array<number>) {
  const maxValue = Math.max(...userData.slice(1, -1));
  const yLabelStep = 100 * Math.ceil(maxValue / 600);

  const startingValue = userData[0] + (userData[0] - userData[1]);
  const endingValue =
    userData[userData.length - 1] -
    (userData[userData.length - 2] - userData[userData.length - 1]);

  userData = [startingValue, ...userData, endingValue];

  const yLabels = userData.slice(1, -1).map((_, index) => index * yLabelStep);

  const xLabels = ["Mon", "Tue", "Wed", "Thu", "Fry", "Sat", "Sun"];

  const graphData = {
    labels: userData.map(() => ""),
    datasets: [
      {
        label: "Calories burned:",
        data: userData,
        borderColor: tailwindColors.violet,
        borderWidth: 1.5,
        pointBackgroundColor: "white",
        backgroundColor: (context: ScriptableContext<"line">) => {
          const ctx = context.chart.ctx;
          const gradient = ctx.createLinearGradient(0, 0, 0, 500);
          gradient.addColorStop(0, tailwindColors.violet);
          gradient.addColorStop(1, "transparent");
          return gradient;
        },
        fill: "start"
      }
    ]
  };

  const graphOptions = {
    scales: {
      x: {
        grid: { display: true, color: "black", offset: true },
        ticks: {
          display: false
        }
      },
      y: {
        display: false,
        grid: { display: true, color: "white", offset: false },
        min: -yLabelStep / 2,
        max: maxValue + 2 * yLabelStep,
        ticks: {
          display: false
        }
      }
    },
    elements: {
      point: {
        radius: 7,
        hitRadius: 7,
        hoverRadius: 7
      }
    },
    plugins: {
      legend: {
        display: false
      },
      filler: {
        propagate: false
      }
    },
    interaction: {
      intersect: true
    }
  };

  return { graphData, graphOptions, xLabels, yLabels };
}
