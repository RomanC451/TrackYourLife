import { ScriptableContext } from "chart.js";
import { colors } from "~/constants/tailwindColors";

export function getGraphConfig(
  userData: Array<number>,
  backgroundHighlighter: unknown,
  onProgress: unknown,
  setSelectedDay: (day: number) => void,
) {
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
        borderColor: colors.violet,
        borderWidth: 1.5,
        pointBackgroundColor: "white",
        backgroundColor: (context: ScriptableContext<"line">) => {
          const ctx = context.chart.ctx;
          const gradient = ctx.createLinearGradient(0, 0, 0, 500);
          gradient.addColorStop(0, colors.violet);
          gradient.addColorStop(1, "transparent");
          return gradient;
        },
        fill: "start",
      },
    ],
  };

  const graphOptions = {
    responsive: true,
    maintainAspectRatio: false,
    animation: {
      onProgress: onProgress,
    },
    devicePixelRatio: 4,
    scales: {
      x: {
        grid: { display: true, color: "black", offset: true },
        ticks: {
          display: false,
        },
      },
      y: {
        display: false,
        grid: { display: true, color: "white", offset: false },
        min: -yLabelStep / 2,
        max: maxValue + 2 * yLabelStep,
        ticks: {
          display: false,
        },
      },
    },
    elements: {
      point: {
        radius: 7,
        hitRadius: 7,
        hoverRadius: 7,
      },
    },
    onClick: (_: unknown, element: [{ index: number }]) => {
      if (element.length > 0) {
        setSelectedDay(element[0].index - 1);
        // you can also get dataset of your selected element
      }
    },
    plugins: {
      legend: {
        display: false,
      },
      filler: {
        propagate: false,
      },
      backgroundHighlighter: backgroundHighlighter,
    },
    interaction: {
      intersect: true,
    },
  };

  return { graphData, graphOptions, xLabels, yLabels };
}

export type graphDataType = {
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    borderColor: string;
    borderWidth: number;
    pointBackgroundColor: string;
    backgroundColor: (context: ScriptableContext<"line">) => CanvasGradient;
    fill: string;
  }[];
};

export type graphOptionsType = {
  responsive: boolean;
  maintainAspectRatio: boolean;
  animation: {
    onProgress: unknown;
  };
  scales: {
    x: {
      grid: {
        display: boolean;
        color: string;
        offset: boolean;
      };
      ticks: {
        display: boolean;
      };
    };
    y: {
      display: boolean;
      grid: {
        display: boolean;
        color: string;
        offset: boolean;
      };
      min: number;
      max: number;
      ticks: {
        display: boolean;
      };
    };
  };
  elements: {
    point: {
      radius: number;
      hitRadius: number;
      hoverRadius: number;
    };
  };
  plugins: {
    legend: {
      display: boolean;
    };
    filler: {
      propagate: boolean;
    };
    backgroundHighlighter: unknown;
  };
  interaction: {
    intersect: boolean;
  };
};
