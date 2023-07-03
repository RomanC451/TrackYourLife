import { useAnimation, useIndexIterator, useWaitCallback } from "~/hooks/";
import { AnimationControls } from "framer-motion";

const cardAnimationProps = {
  transition: { type: "spring", duration: 1.5 }
};

const cardAnimationSteps = [{ x: 345 }, { x: 0 }];

const useCardAnimation = (
  startingStep: number
): [AnimationControls, (callback: Function) => void] => {
  const [canStartAnimation, resetAnimationTimer] = useWaitCallback();
  const [currentStep, nextStep, prevStep] = useIndexIterator(
    cardAnimationSteps.length,
    startingStep
  );
  const [cardAnimationRef, startAnimation] = useAnimation(
    cardAnimationSteps,
    cardAnimationProps
  );

  function startCardAnimation(callback: Function): void {
    if (!canStartAnimation()) {
      return;
    }

    startAnimation(currentStep.current, () => {
      callback();
      nextStep();
      resetAnimationTimer();
    });
  }
  return [cardAnimationRef, startCardAnimation];
};

export default useCardAnimation;
