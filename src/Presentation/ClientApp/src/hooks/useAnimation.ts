import { AnimationControls, useAnimationControls } from "framer-motion";

const skipAnimationProps = {
  transition: { type: "just", duration: 0 }
};

/**
 * Description - Creates the animation controler with desired props and provides the animatio9n start function.
 * @param animationSteps - An array of animation steps, where each step is an object with the animation properties
 * @param defaultProps - The default props for the animation
 * @returns the animation controler ref and the animation start function
 */

const useAnimation = (
  animationSteps: Array<{ [key: string]: any }>,
  defaultProps = {}
): [
  AnimationControls,
  (
    currentAnimationStep: number,
    callback?: Function,
    skipAnimation?: boolean
  ) => Promise<void>
] => {
  const animationRef = useAnimationControls();

  /**
   * Starts an animation with the specified step and callback function.
   *
   * @param currentAnimationStep - The current step of the animation.
   * @param [callback=null] - A callback function to be executed after the animation is finished.
   *
   * @returns
   */
  async function startAnimation(
    currentAnimationStep: number,
    callback: Function = () => {},
    skipAnimation: boolean = false
  ) {
    const defProps = skipAnimation ? skipAnimationProps : defaultProps;

    let props = {
      ...defProps,
      ...animationSteps[currentAnimationStep]
    };

    await animationRef.start(props).then(() => {
      callback();
    });
  }

  return [animationRef, startAnimation];
};

export default useAnimation;
