import { AnimatePresence, motion, wrap } from "framer-motion";
import React, { forwardRef, useImperativeHandle, useState } from "react";
import { IoIosArrowDropleft, IoIosArrowDropright } from "react-icons/io";

const animationVariants = {
  enter: (direction: number) => {
    return {
      x: direction > 0 ? 1000 : -1000
    };
  },
  center: {
    zIndex: 1,
    x: 0
  },
  exit: (direction: number) => {
    return {
      zIndex: 0,
      x: direction < 0 ? 1000 : -1000
    };
  }
};

interface SliderProps {
  pages: JSX.Element[];
  ref?: React.Ref<SliderPropsRef>;
}

export interface SliderPropsRef {
  goToPage: (labelName: number) => void;
}

const Slider: React.FC<SliderProps> = forwardRef<SliderPropsRef, SliderProps>(
  ({ pages }, ref): JSX.Element => {
    const [[page, direction], setPage] = useState([0, 0]);

    const paginate = (newDirection: number) => {
      const newPage = Math.max(
        Math.min(page + newDirection, pages.length - 1),
        0
      );
      setPage([newPage, newDirection]);
    };

    const pageIndex = wrap(0, pages.length, page);

    function goToPage(pageNr: number) {
      const newPage = Math.max(Math.min(pageNr, pages.length - 1), 0);
      if (page === pageNr) return;

      const direction = page - newPage > 0 ? -1 : 1;
      setPage([newPage, direction]);
    }

    useImperativeHandle(ref, () => ({
      goToPage
    }));

    return (
      <>
        <div className="w-[304px] h-[290px] relative flex  overflow-hidden justify-center items-center">
          <AnimatePresence initial={false} custom={direction}>
            <motion.div
              key={page}
              custom={direction}
              variants={animationVariants}
              initial="enter"
              animate="center"
              exit="exit"
              transition={{
                x: { type: "spring", stiffness: 200, damping: 20 }
                // opacity: { duration: 0.2 }
              }}
            >
              <div className="absolute w-[304px] top-[50%] left-[50%] translate-x-[-50%] translate-y-[-50%]">
                {pages[pageIndex]}
              </div>
            </motion.div>
          </AnimatePresence>
        </div>
        <div className="flex w-[100%] justify-end gap-2 mb-[3px]">
          <div className="flex h-[35px] w-[35px] items-center justify-center rounded-full hover:bg-slate-100 hover:shadow-lg z-10">
            <button
              type="button"
              onClick={() => paginate(-1)}
              className="text-3xl"
              style={{ color: "gray" }}
            >
              <IoIosArrowDropleft />
            </button>
          </div>
          <div className="flex h-[35px] w-[35px] items-center justify-center rounded-full hover:bg-slate-100 hover:shadow-lg z-10">
            <button
              type="button"
              onClick={() => paginate(1)}
              className="text-3xl"
              style={{ color: "gray" }}
            >
              <IoIosArrowDropright />
            </button>
          </div>
        </div>
      </>
    );
  }
);

export default Slider;
