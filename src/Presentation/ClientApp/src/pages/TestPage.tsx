import "./testPage.css";

import { AnimatePresence, motion } from "framer-motion";
import { wrap } from "popmotion";
import { useState } from "react";

const variants = {
  enter: (direction: number) => {
    return {
      x: direction > 0 ? 100000 : -100000
    };
  },
  center: {
    zIndex: 1,
    x: 0
  },
  exit: (direction: number) => {
    return {
      zIndex: 0,
      x: direction < 0 ? 100000 : -100000
    };
  }
};

const divs = [
  <div className="element"> 1</div>,
  <div className="element"> 2</div>,
  <div className="element"> 3</div>
];

const TestPage = () => {
  const [[page, direction], setPage] = useState([0, 0]);

  const imageIndex = wrap(0, divs.length, page);

  const paginate = (newDirection: number) => {
    setPage([page + newDirection, newDirection]);
  };

  return (
    <div className="example-container">
      <AnimatePresence initial={false} custom={direction}>
        <motion.div
          key={page}
          // src={images[imageIndex]}
          custom={direction}
          variants={variants}
          initial="enter"
          animate="center"
          exit="exit"
          transition={{
            x: { type: "spring", stiffness: 200, damping: 20 },
            opacity: { duration: 0.2 }
          }}
        >
          {divs[imageIndex]}
        </motion.div>
      </AnimatePresence>
      <div className="next" onClick={() => paginate(1)}>
        {"‣"}
      </div>
      <div className="prev" onClick={() => paginate(-1)}>
        {"‣"}
      </div>
    </div>
  );
};

export default TestPage;
