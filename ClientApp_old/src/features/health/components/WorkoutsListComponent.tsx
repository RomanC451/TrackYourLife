import React from "react";
import { StarSvg } from "~/assets";
import { CurvedDotedLineSvg, CurvedLineSvg } from "~/assets/health";
import {
  AbsPng,
  ArmsPng,
  BackPng,
  ChestPng,
  LegsPng,
} from "~/assets/health/workouts";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { colors } from "~/constants/tailwindColors";

const workouts = [
  {
    name: "Arms",
    color: colors.green,
    textColor: "white",
    picture: ArmsPng,
  },
  {
    name: "Back",
    color: colors.turquoise,
    textColor: "black",
    picture: BackPng,
  },
  {
    name: "Chest",
    color: colors.red,
    textColor: "white",
    picture: ChestPng,
  },
  {
    name: "Abs",
    color: colors.yellow,
    textColor: "black",
    picture: AbsPng,
  },
  {
    name: "Legs",
    color: colors.violet,
    textColor: "white",
    picture: LegsPng,
  },
];

const WorkoutsListComponent: React.FC = (): JSX.Element => {
  return (
    <BoxStyledComponent
      title="Workouts"
      className="flex flex-wrap items-center justify-around gap-[50px] pb-[25px] pl-[20px] pr-[20px] pt-[25px]"
    >
      {/* <ComponentTopBarMenuLayout title="Workouts">
      <div className="relative bg-second-gray-bg  flex-shrink-0 rounded-[10px] shadow-[0_4px_4px_0_rgba(0,0,0,0.25)_inset] pl-[20px] flex flex-wrap gap-[50px] justify-around items-center pt-[25px] pb-[25px] pr-[20px]"> */}
      {workouts.map((workout, index) => {
        // const gradientStyle = {
        //   backgroundColor: "linear-gradient(yellow,lightgreen)",
        // };
        //   #7AAF76, #6F4BDA00
        return (
          <div
            key={index}
            style={{
              backgroundImage: `linear-gradient(360deg, ${workout.color},rgba(111, 75, 218, 0.00))`,
              color: workout.textColor,
            }}
            className="relative h-[183px] w-[183px] rounded-[15px]"
          >
            <div className="mt-[50%]">
              <CurvedLineSvg />
              <CurvedDotedLineSvg />
              {/* className="mt-[-30px]" /> */}
            </div>
            <img
              src={workout.picture}
              className="absolute right-[-5px] top-0"
            />
            <div
              style={{
                backgroundImage: `linear-gradient(360deg, ${workout.color},rgba(111, 75, 218, 0.00)35%)`,
                color: workout.textColor,
              }}
              className="absolute left-0 top-0 h-[183px] w-[183px] rounded-[15px]"
            >
              <div className="ml-[10px] mt-[42px] flex flex-col">
                <p className=" text-[20px] font-bold">{workout.name}</p>
                <p className=" text-[20px] font-bold">Workout</p>
              </div>
              <div className="mt-[30px] flex items-center justify-around">
                <div
                  style={{
                    backgroundColor: workout.color,
                    filter: "drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.25))",
                  }}
                  className="h-[35px] w-[35px] rounded-full"
                >
                  <StarSvg />
                  {/* fill={workout.textColor} /> */}
                </div>
                <p className=" font-bold underline">View workout</p>
              </div>
            </div>
          </div>
        );
      })}
    </BoxStyledComponent>
  );
};

export default WorkoutsListComponent;
