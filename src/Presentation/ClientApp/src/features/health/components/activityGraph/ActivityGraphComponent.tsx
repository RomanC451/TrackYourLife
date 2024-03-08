import {
  CategoryScale,
  Chart,
  Filler,
  Legend,
  LinearScale,
  LineElement,
  PointElement,
  Title,
  Tooltip,
} from "chart.js";
import React, { useEffect } from "react";
import GrowingModal from "~/animations/GrowingModal";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { colors } from "~/constants/tailwindColors";
import { screensEnum } from "~/constants/tailwindSizes";
import useActivityGraph from "~/features/health/hooks/useActivityGraph";

const ActivityGraphComponent: React.FC = (): JSX.Element => {
  const {
    selectedDay,
    screenSize,
    xLabels,
    yLabels,
    // graphData,
    // graphOptions,
    backgroundHighlighter,
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
      backgroundHighlighter,
    );
  }, []);

  return (
    <GrowingModal
      maxWidth={800}
      maxHeight={500}
      minWidth={screenSize.width <= screensEnum.sm ? 300 : 560}
      minHeight={333}
    >
      <BoxStyledComponent className="flex gap-5" title="Activities">
        <div className=" text-gray mb-[-2px] ml-[20px] flex h-[280px] flex-grow-0 flex-col justify-end gap-[15px] text-[15px] font-bold">
          {yLabels.reverse().map((value, index) => {
            return (
              <p key={index} className="inline-block w-[28px] text-center ">
                {value}
              </p>
            );
          })}
        </div>
        <div
          className="relative grid h-[290px] w-[calc(100%-(100px))]   place-items-center overflow-hidden"
          onClick={(e) => {
            e.preventDefault();
          }}
        >
          <div className=" absolute grid h-[300px] w-[120%] place-items-center lg:w-[117%]">
            <div className=" absolute left-[5%px] top-[15px] flex  w-[calc(100%-100px)] justify-around text-[15px] font-bold">
              {xLabels.map((value, index) => {
                return (
                  <p
                    style={{
                      color: index === selectedDay ? "white" : colors.gray,
                    }}
                    key={index}
                    className="inline-block w-[62px] text-center "
                  >
                    {value}
                  </p>
                );
              })}
            </div>

            {/* <Line data={graphData} options={graphOptions} className="" /> */}
          </div>
        </div>
      </BoxStyledComponent>
    </GrowingModal>
  );
};

export default ActivityGraphComponent;
