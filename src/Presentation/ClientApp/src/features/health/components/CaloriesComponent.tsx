import { AnimatePresence, motion } from "framer-motion";
import React, { useEffect, useRef, useState } from "react";
import GrowingModal, { GrowingModalRef } from "~/animations/GrowingModal";
import { CaloriesPlusSvg, DottedSemiCircleBorderSvg } from "~/assets/health";
import BoxStyledComponent from "~/components/BoxStyledComponent";
import { CircleProgressBar } from "~/components/progressBars";
import { tailwindColors } from "~/constants/tailwindColors";
import AbsoluteCenterChildrenLayout from "~/layouts/AbsoluteCenterChildrenLayout";

const CaloriesComponent: React.FC = (): JSX.Element => {
  const [completition, setCompletition] = useState(34);

  return (
    <GrowingModal maxWidth={500} maxHeight={500} minWidth={322} minHeight={195}>
      <BoxStyledComponent
        minWidth={322}
        height={0}
        title="Calories"
        className="flex-grow"
      >
        <AbsoluteCenterChildrenLayout className="pt-[10px]">
          <CircleProgressBar
            color={tailwindColors["violet"]}
            darkColor={tailwindColors["dark-violet"]}
            completitionPercentage={completition}
          />
        </AbsoluteCenterChildrenLayout>
        <AbsoluteCenterChildrenLayout className="pt-[54.5px]">
          <DottedSemiCircleBorderSvg />
        </AbsoluteCenterChildrenLayout>
        <AbsoluteCenterChildrenLayout className="pt-[81.5px] z-10">
          <button onClick={() => setCompletition(completition + 10)}>
            <CaloriesPlusSvg />
          </button>
        </AbsoluteCenterChildrenLayout>
        <AbsoluteCenterChildrenLayout className="pt-[137px] ">
          <div className="font-semibold flex flex-col items-center gap-[10px]">
            <p className="text-white  text-[24px]  leading-[26.4px]">
              {completition}%
            </p>
            <p className="text-gray  text-[14px]  leading-[15.4px]">
              Today's calories
            </p>
          </div>
        </AbsoluteCenterChildrenLayout>

        {/* <motion.button
          key="modalButton"
          className={`absolute right-0 bg-rose-800 w-[30px] h-[30px] rounded-full mt-[10px] mr-[10px] hover:bg-rose-200 z-10 ${
            modalActive ? "" : "disabled"
          }`}
          initial={{ opacity: 0 }}
          transition={{ duration: 1 }}
          animate={modalActive ? { opacity: 1 } : { opacity: 0 }}
          exit={{
            opacity: 0,
            transition: { duration: 1 }
          }}
          onClick={() => modalRef.current?.closeModal()}
        >
          X
        </motion.button> */}
      </BoxStyledComponent>
    </GrowingModal>
  );
};

export default CaloriesComponent;
