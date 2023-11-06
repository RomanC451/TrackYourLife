import React from "react";
import { StarSvg } from "~/assets";
import { CurvedDotedLineSvg, CurvedLineSvg } from "~/assets/health";
import {
  AbsPng,
  ArmsPng,
  BackPng,
  ChestPng,
  LegsPng
} from "~/assets/health/workouts";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { tailwindColors } from "~/constants/tailwindColors";
import ComponentTopBarMenuLayout from "~/layouts/ComponentTopBarMenuLayout";

const workouts = [
  {
    name: "Arms",
    color: tailwindColors.green,
    textColor: "white",
    picture: ArmsPng
  },
  {
    name: "Back",
    color: tailwindColors.turquoise,
    textColor: "black",
    picture: BackPng
  },
  {
    name: "Chest",
    color: tailwindColors.red,
    textColor: "white",
    picture: ChestPng
  },
  {
    name: "Abs",
    color: tailwindColors.yellow,
    textColor: "black",
    picture: AbsPng
  },
  {
    name: "Legs",
    color: tailwindColors.violet,
    textColor: "white",
    picture: LegsPng
  }
];

const fontStyle = "font-[Nunito_Sans] ";

const WorkoutsListComponent: React.FC = (): JSX.Element => {
  return (
    <BoxStyledComponent
      minWidth={300}
      height={195}
      title="Workouts"
      className="pl-[20px] flex flex-wrap gap-[50px] justify-around items-center pt-[25px] pb-[25px] pr-[20px]"
    >
      {/* <ComponentTopBarMenuLayout title="Workouts">
      <div className="relative bg-second-gray-bg  flex-shrink-0 rounded-[10px] shadow-[0_4px_4px_0_rgba(0,0,0,0.25)_inset] pl-[20px] flex flex-wrap gap-[50px] justify-around items-center pt-[25px] pb-[25px] pr-[20px]"> */}
      {workouts.map((workout, index) => {
        const gradientStyle = {
          backgroundColor: "linear-gradient(yellow,lightgreen)"
        };
        //   #7AAF76, #6F4BDA00
        return (
          <div
            key={index}
            style={{
              backgroundImage: `linear-gradient(360deg, ${workout.color},rgba(111, 75, 218, 0.00))`,
              color: workout.textColor
            }}
            className="relative w-[183px] h-[183px] rounded-[15px]"
          >
            <div className="mt-[50%]">
              <CurvedLineSvg />
              <CurvedDotedLineSvg className="mt-[-30px]" />
            </div>
            <img
              src={workout.picture}
              className="right-[-5px] top-0 absolute"
            />
            <div
              style={{
                backgroundImage: `linear-gradient(360deg, ${workout.color},rgba(111, 75, 218, 0.00)35%)`,
                color: workout.textColor
              }}
              className="absolute top-0 left-0 w-[183px] h-[183px] rounded-[15px]"
            >
              <div className="flex flex-col ml-[10px] mt-[42px]">
                <p className=" text-[20px] font-bold">{workout.name}</p>
                <p className=" text-[20px] font-bold">Workout</p>
              </div>
              <div className="mt-[30px] flex items-center justify-around">
                <div
                  style={{
                    backgroundColor: workout.color,
                    filter: "drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.25))"
                  }}
                  className="w-[35px] h-[35px] rounded-full"
                >
                  <StarSvg fill={workout.textColor} />
                </div>
                <p className=" underline font-bold">View workout</p>
              </div>
            </div>
          </div>
        );
      })}
    </BoxStyledComponent>
  );
};

export default WorkoutsListComponent;
