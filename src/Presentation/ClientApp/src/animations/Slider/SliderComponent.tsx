import { motion } from "framer-motion";
import React, {
  forwardRef,
  useImperativeHandle,
  useMemo,
  useRef,
  useState
} from "react";
import { sliderComponentPosition } from "~/data/positionEnums";
import { useAnimation, useIndexIterator } from "~/hooks";

const sliderAnimationProps = {
  transition: { type: "spring", duration: 1 }
};

export interface SliderComponentRef {
  startSlideAnimation: (
    direction: sliderComponentPosition,
    callback: Function,
    force?: boolean
  ) => void;
}

export interface SliderComponentProps {
  component: React.ReactNode;
  sliderWidth: number;
  pos: sliderComponentPosition;
  ref: React.Ref<any>;
}

const SliderComponent: React.FC<SliderComponentProps> = forwardRef<
  SliderComponentRef,
  SliderComponentProps
>(({ component, sliderWidth, pos }, ref): JSX.Element => {
  const sliderAnimationSteps = [
    { x: -sliderWidth },
    { x: 0 },
    { x: sliderWidth }
  ];

  const initialPosRef = useRef(
    pos === sliderComponentPosition.right ? sliderWidth : 0
  );

  const currentStepRef = useRef(pos === sliderComponentPosition.right ? 2 : 1);

  const [currentIndex, nextStep, prevStep] = useIndexIterator(
    sliderAnimationSteps.length,
    currentStepRef.current,
    false
  );

  const [slideAnimationRef, startAnimation] = useAnimation(
    sliderAnimationSteps,
    sliderAnimationProps
  );

  function startSlideAnimation(
    direction: sliderComponentPosition,
    callback: Function,
    force: boolean = false
  ) {
    if (direction === sliderComponentPosition.right) {
      if (force) currentIndex.current = sliderAnimationSteps.length - 1;
      else nextStep();
    } else if (direction === sliderComponentPosition.left) {
      if (force) currentIndex.current = 0;
      else prevStep();
    }

    startAnimation(
      currentIndex.current,
      () => {
        callback();
        initialPosRef.current = sliderAnimationSteps[currentIndex.current].x;
      },
      force
    );
  }

  useImperativeHandle(ref, () => ({
    startSlideAnimation
  }));

  return (
    <motion.div
      className={`absolute top-0  float-left flex w-full flex-col items-center`}
      transition={{ duration: 5 }}
      animate={slideAnimationRef}
      initial={{ x: initialPosRef.current }}
    >
      {component}
    </motion.div>
  );
});

export default SliderComponent;
