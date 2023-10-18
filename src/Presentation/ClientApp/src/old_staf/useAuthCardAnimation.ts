import { AnimationControls } from "framer-motion";
import { useAnimation, useIndexIterator, useWaitCallback } from "~/hooks/";

const cardAnimationProps = {
  transition: { type: "spring", duration: 1.5 }
};

const cardAnimationSteps = [{ x: 370 }, { x: 30 }];

const useAuthCardAnimation = (
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

export default useAuthCardAnimation;
