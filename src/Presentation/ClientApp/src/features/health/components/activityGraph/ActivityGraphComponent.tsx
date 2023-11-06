import {
  CategoryScale,
  Chart,
  Filler,
  Legend,
  LinearScale,
  LineElement,
  PointElement,
  Title,
  Tooltip
} from "chart.js";
import React, { useEffect } from "react";
import { Line } from "react-chartjs-2";
import GrowingModal from "~/animations/GrowingModal";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { tailwindColors } from "~/constants/tailwindColors";
import { screensEnum } from "~/constants/tailwindSizes";
import useActivityGraph from "~/features/health/hooks/useActivityGraph";

const ActivityGraphComponent: React.FC = (): JSX.Element => {
  const {
    selectedDay,
    screenSize,
    xLabels,
    yLabels,
    graphData,
    graphOptions,
    backgroundHighlighter,
    setSelectedDay
  } = useActivityGraph();

  useEffect(() => {
    Chart.register(
      CategoryScale,
      LinearScale,
      PointElement,
      LineElement,
      Title,
      Tooltip,
      Legend,
      Filler,
      backgroundHighlighter
    );
  }, []);
  return (
    <GrowingModal
      maxWidth={800}
      maxHeight={500}
      minWidth={screenSize.width <= screensEnum.sm ? 300 : 560}
      minHeight={300}
    >
      <BoxStyledComponent
        className="flex gap-5"
        minWidth={screenSize.width <= screensEnum.sm ? 300 : 560}
        height={272}
        title="Activities"
      >
        <div className=" flex flex-col flex-grow-0 mt-[14%] text-gray text-[15px] font-bold  gap-[7px]">
          {yLabels.reverse().map((value, index) => {
            return (
              <p key={index} className="text-center w-[28px] inline-block ">
                {value}
              </p>
            );
          })}
        </div>
        <div className="relative grid place-items-center overflow-hidden   w-[calc(100%-(100px))] h-[300px]">
          <div className=" absolute h-[300px]  grid place-items-center w-[120%] lg:w-[117%]">
            <Line data={graphData} options={graphOptions} />
          </div>
        </div>

        <div className=" absolute top-[15px] left-[85px] flex  text-[15px] font-bold w-[calc(100%-100px)] justify-around">
          {xLabels.map((value, index) => {
            return (
              <p
                style={{
                  color: index === selectedDay ? "white" : tailwindColors.gray
                }}
                key={index}
                className="text-center w-[62px] inline-block "
              >
                {value}
              </p>
            );
          })}
        </div>
        {/* <button onClick={() => setSelectedDay(selectedDay + 1)}>next</button> */}
      </BoxStyledComponent>
    </GrowingModal>
  );
};

export default ActivityGraphComponent;
