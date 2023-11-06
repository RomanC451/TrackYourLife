import { useRef, useState } from "react";
import { tailwindColors } from "~/constants/tailwindColors";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import {
  getGraphConfig,
  graphDataType,
  graphOptionsType
} from "~/features/health/components/activityGraph/graphConfig";

interface ActivityGraphData {
  selectedDay: number;
  screenSize: { width: number; height: number };
  graphData: graphDataType;
  graphOptions: graphOptionsType;
  yLabels: number[];
  xLabels: string[];
  backgroundHighlighter: {
    id: string;
    beforeDatasetsDraw(chart: any, args: any, options: any): void;
  };
  setSelectedDay: (day: number) => void;
}

const useActivityGraph = (): ActivityGraphData => {
  const { screenSize } = useAppGeneralStateContext();

  const [selectedDay, setSelectedDay] = useState(2);

  const updateSelectedDay = (day: number) => {
    setSelectedDay(day);
    selectedDayRef.current = day;
  };
  const selectedDayRef = useRef(selectedDay);

  const animationPercentageRef = useRef(0);

  const animationFinsihed = useRef(false);

  let userData = [350, 200, 350, 200, 600, 220, 300];

  const backgroundHighlighter = {
    id: "backgroundHighlighter",
    beforeDatasetsDraw(chart: any, args: any, options: any) {
      const {
        ctx,
        chartArea: { bottom, height },
        scales: { x, y }
      } = chart;
      if (!ctx || !x || !y || !x._gridLineItems) return;
      if (animationFinsihed.current) {
        animationPercentageRef.current = 100;
      }
      if (animationPercentageRef.current === 100) {
        animationFinsihed.current = true;
      }
      const gradient = ctx.createLinearGradient(0, 0, 0, 500);
      gradient.addColorStop(0, tailwindColors.violet);
      gradient.addColorStop(1, "transparent");
      const newWidth = x._gridLineItems[1].x1 - x._gridLineItems[0].x1;
      var newHeight = height * (animationPercentageRef.current / 100);

      if (newHeight > 290) newHeight = 290;

      ctx.fillStyle = gradient;
      var x1 = x._gridLineItems[selectedDayRef.current].x1;
      var y1 = bottom - newHeight;
      var w = newWidth;
      var h = newHeight;
      var r = 10;
      var x2 = x1 + w;
      var y2 = y1 + h;
      if (w < 2 * r) r = w / 2;
      if (h < 2 * r) r = h / 2;
      ctx.beginPath();
      ctx.moveTo(x1 + r, y1);
      ctx.arcTo(x2, y1, x2, y2, r);
      ctx.arcTo(x2, y2, x1, y2, r);
      ctx.arcTo(x1, y2, x1, y1, r);
      ctx.arcTo(x1, y1, x2, y1, r);
      ctx.closePath();
      ctx.fill();
    }
  };

  const onProgress = (context: any) => {
    animationPercentageRef.current = Math.round(
      (context.currentStep / (context.numSteps - 50)) * 100
    );
    if (animationPercentageRef.current > 95)
      animationPercentageRef.current = 100;

    selectedDayRef.current = selectedDay;
  };

  const { graphData, graphOptions, yLabels, xLabels } = getGraphConfig(
    userData,
    backgroundHighlighter,
    onProgress,
    setSelectedDay
  );

  return {
    selectedDay,
    screenSize,
    graphData,
    graphOptions,
    yLabels,
    xLabels,
    backgroundHighlighter,
    setSelectedDay: updateSelectedDay
  };
};

export default useActivityGraph;
