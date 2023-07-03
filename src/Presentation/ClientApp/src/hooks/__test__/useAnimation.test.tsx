import { render, renderHook, screen } from "@testing-library/react";
import React from "react";

import { motion, AnimationControls } from "framer-motion";

import useAnimation from "../useAnimation";

interface MockedElementProps {
  animationRef: AnimationControls;
  initialPos: { x?: number; y?: number; z?: number; rotate?: number };
  id: number;
}

const MockElement: React.FC<MockedElementProps> = ({
  animationRef,
  initialPos,
  id
}): JSX.Element => {
  return (
    <motion.div
      animate={animationRef}
      data-testid={`animated-div-${id}`}
      initial={initialPos}
    ></motion.div>
  );
};

function getHookReturns(animationSteps: Array<{ [key: string]: any }>) {
  const { result } = renderHook(() => useAnimation(animationSteps));
  return result.current;
}

test("animation ref should be defined correctly", () => {
  const [animationRef, startAnimation] = getHookReturns([{ x: -200 }]);
  expect(animationRef.hasOwnProperty("subscribe")).toBe(true);
  expect(animationRef.hasOwnProperty("start")).toBe(true);
  expect(animationRef.hasOwnProperty("set")).toBe(true);
  expect(animationRef.hasOwnProperty("stop")).toBe(true);
  expect(animationRef.hasOwnProperty("mount")).toBe(true);
});

var testId = 0;

describe("vertical and horizontal movement", () => {
  function setupElement(id: number, animationRef: AnimationControls) {
    render(
      <MockElement
        animationRef={animationRef}
        initialPos={{ x: -200, y: -200 }}
        id={id}
      />
    );
  }

  test("1", async () => {
    const id = testId++;
    const [animationRef, startAnimation] = getHookReturns([
      { x: -200, y: -200 },
      { x: 0, y: 0 },
      { x: 200, y: 200 }
    ]);
    setupElement(id, animationRef);

    const divElement = screen.getByTestId(`animated-div-${id}`);

    expect(divElement.style.transform).toBe(
      "translateX(-200px) translateY(-200px) translateZ(0)"
    );

    await startAnimation(1, () => {
      expect(divElement.style.transform).toBe("none");
    });
  });

  test("2", async () => {
    const id = testId++;
    const [animationRef, startAnimation] = getHookReturns([
      { x: -200, y: -200 },
      { x: 0, y: 0 },
      { x: 200, y: 200 }
    ]);
    setupElement(id, animationRef);

    const divElement = screen.getByTestId(`animated-div-${id}`);

    await startAnimation(2, () => {
      expect(divElement.style.transform).toBe(
        "translateX(200px) translateY(200px) translateZ(0)"
      );
    });

    await startAnimation(0, () => {
      expect(divElement.style.transform).toBe(
        "translateX(-200px) translateY(-200px) translateZ(0)"
      );
    });
  });

  describe("rotation", () => {
    function setupElement(id: number, animationRef: AnimationControls) {
      render(
        <MockElement
          animationRef={animationRef}
          initialPos={{ rotate: 150 }}
          id={id}
        />
      );
    }

    test("1", async () => {
      const id = testId++;

      const [animationRef, startAnimation] = getHookReturns([
        { rotate: 150 },
        { rotate: 0 },
        { rotate: 360 }
      ]);
      setupElement(id, animationRef);

      const divElement = screen.getByTestId(`animated-div-${id}`);

      expect(divElement.style.transform).toBe("rotate(150deg) translateZ(0)");

      await startAnimation(1, () => {
        expect(divElement.style.transform).toBe("none");
      });
    });

    test("2", async () => {
      const id = testId++;

      const [animationRef, startAnimation] = getHookReturns([
        { rotate: 150 },
        { rotate: 0 },
        { rotate: 360 }
      ]);
      setupElement(id, animationRef);
      const divElement = screen.getByTestId(`animated-div-${id}`);

      await startAnimation(2, () => {
        expect(divElement.style.transform).toBe("rotate(360deg) translateZ(0)");
      });

      await startAnimation(0, () => {
        expect(divElement.style.transform).toBe("rotate(150deg) translateZ(0)");
      });
    });
  });
});
