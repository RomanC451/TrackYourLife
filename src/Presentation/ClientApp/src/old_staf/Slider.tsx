import React, { forwardRef, useImperativeHandle, useMemo, useRef } from "react";
import { IoIosArrowDropleft, IoIosArrowDropright } from "react-icons/io";
import { sliderComponentPosition as compPos } from "~/data/positionEnums";
import { useWaitCallback } from "~/hooks";

import SliderComponent, { SliderComponentRef } from "./SliderComponent";

interface SliderProps {
  width: number;
  className?: string;
  sliderComponents: Array<React.ReactNode>;
  ref: React.Ref<SliderRef>;
}

export interface SliderRef {
  goToSliderComponent: (componentIndexToBeDesplayed: number) => void;
}

/**

A slider component that displays a sequence of slider components.
@param width The width of the slider.
@param className Optional class name for custom styling.
@param sliderComponents An array of React nodes representing the slider components to display.
@param ref The reference to the component
@returns A React component representing the slider.
*/
const Slider: React.FC<SliderProps> = forwardRef<SliderRef, SliderProps>(
  ({ width, className, sliderComponents }, ref): JSX.Element => {
    useImperativeHandle(ref, () => ({
      goToSliderComponent
    }));

    const [canStartAnimation, resetAnimationTimer] = useWaitCallback();
    const waitingForAnimation = useRef(false);
    const currentTheoreticComponentIndex = useRef(0);
    const currentActualComponentIndex = useRef(0);

    const componentsRef = useMemo(
      () =>
        sliderComponents.map(() =>
          React.createRef<SliderComponentRef>()
        ) as React.MutableRefObject<SliderComponentRef | undefined>[],
      [sliderComponents]
    );

    function goToSliderComponent(componentIndexToBeDesplayed: number) {
      const animationCanNotBeStarted =
        waitingForAnimation.current ||
        componentIndexToBeDesplayed > componentsRef.length - 1 ||
        componentIndexToBeDesplayed < 0 ||
        componentIndexToBeDesplayed === currentTheoreticComponentIndex.current;
      if (animationCanNotBeStarted) return;
      currentTheoreticComponentIndex.current = componentIndexToBeDesplayed;
      startAnimation(componentIndexToBeDesplayed);
    }

    function startAnimation(componentIndexToBeDesplayed: number) {
      if (!canStartAnimation()) {
        waitingForAnimation.current = true;
        setTimeout(() => {
          startAnimation(componentIndexToBeDesplayed);
        }, 100); // Adjust the delay time as needed
        return;
      }

      waitingForAnimation.current = false;

      if (componentIndexToBeDesplayed === currentActualComponentIndex.current) {
        return;
      }

      componentsRef.forEach((comp, index) => {
        if (index === componentIndexToBeDesplayed) {
          const direction =
            componentIndexToBeDesplayed > currentActualComponentIndex.current
              ? compPos.left
              : compPos.right;
          comp.current?.startSlideAnimation(direction, () => {
            resetAnimationTimer();
            currentActualComponentIndex.current = componentIndexToBeDesplayed;
          });
        } else if (index === currentActualComponentIndex.current) {
          const direction =
            componentIndexToBeDesplayed > currentActualComponentIndex.current
              ? compPos.left
              : compPos.right;
          comp.current?.startSlideAnimation(direction, () => {});
        } else if (index < componentIndexToBeDesplayed)
          comp.current?.startSlideAnimation(compPos.left, () => {}, true);
        else if (index > componentIndexToBeDesplayed)
          comp.current?.startSlideAnimation(compPos.right, () => {}, true);
      });
    }

    return (
      <div className={`relative overflow-hidden ${className} flex items-end`}>
        {sliderComponents.map((component, index) => (
          <SliderComponent
            key={index}
            pos={
              index === currentActualComponentIndex.current
                ? compPos.center
                : index > currentActualComponentIndex.current
                ? compPos.right
                : compPos.left
            }
            sliderWidth={width}
            component={component}
            ref={componentsRef[index]}
          ></SliderComponent>
        ))}
        <div className="flex w-[100%] justify-end gap-2 mb-[3px]">
          <div className="flex h-[35px] w-[35px] items-center justify-center rounded-full hover:bg-slate-100 hover:shadow-lg z-10">
            <button
              type="button"
              onClick={() =>
                goToSliderComponent(currentTheoreticComponentIndex.current - 1)
              }
              className="text-3xl"
              style={{ color: "gray" }}
            >
              <IoIosArrowDropleft />
            </button>
          </div>
          <div className="flex h-[35px] w-[35px] items-center justify-center rounded-full hover:bg-slate-100 hover:shadow-lg z-10">
            <button
              type="button"
              onClick={() =>
                goToSliderComponent(currentTheoreticComponentIndex.current + 1)
              }
              className="text-3xl"
              style={{ color: "gray" }}
            >
              <IoIosArrowDropright />
            </button>
          </div>
        </div>
      </div>
    );
  }
);

export default Slider;
