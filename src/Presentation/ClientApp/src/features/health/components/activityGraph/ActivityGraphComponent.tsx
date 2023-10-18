import {
  CategoryScale,
  Chart as ChartJS,
  Filler,
  Legend,
  LinearScale,
  LineElement,
  PointElement,
  Title,
  Tooltip
} from "chart.js";
import { motion } from "framer-motion";
import React, { useState } from "react";
import { Line } from "react-chartjs-2";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { tailwindColors } from "~/constants/tailwindColors";
import { getGraphConfig } from "~/features/health/components/activityGraph/graphConfig";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
);

const ActivityGraphComponent: React.FC = (): JSX.Element => {
  const [activeYLabel, setActiveYLabel] = useState(3);

  let userData = [350, 200, 350, 200, 600, 220, 300];

  const { graphData, graphOptions, yLabels, xLabels } =
    getGraphConfig(userData);

  return (
    <BoxStyledComponent
      className="grid place-items-center"
      width={577}
      height={272}
      title="Activities"
    >
      <motion.div
        initial={{ opacity: 0, height: 0 }}
        animate={{ opacity: 1, height: 247 }}
        transition={{ duration: 0.5 }}
        style={{ left: 277 }}
        className="absolute bottom-[10px] w-[63px] h-[247px] bg-violet/50 rounded-[10px_10px_0_0]"
      />
      <div className="relative grid place-items-center overflow-hidden w-[450px] h-full bottom-[10px] left-[20px]">
        <div className="w-[530px] absolute h-[300px] top-[20px] grid place-items-center">
          <Line data={graphData} options={graphOptions} />
        </div>
      </div>

      <div className="absolute bottom-[8px] left-[40px] flex flex-col text-gray text-[15px] font-bold  gap-[7px]">
        {yLabels.reverse().map((value, index) => {
          return (
            <p key={index} className="text-center w-[28px] inline-block ">
              {value}
            </p>
          );
        })}
      </div>
      <div className=" absolute top-[15px] left-[85px] flex  text-[15px] font-bold  gap-[2px]">
        {xLabels.map((value, index) => {
          return (
            <p
              style={{
                color: index === activeYLabel ? "white" : tailwindColors.gray
              }}
              key={index}
              className="text-center w-[62px] inline-block "
            >
              {value}
            </p>
          );
        })}
      </div>
    </BoxStyledComponent>
  );
};

export default ActivityGraphComponent;
