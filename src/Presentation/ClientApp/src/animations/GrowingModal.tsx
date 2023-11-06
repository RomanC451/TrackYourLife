import { AnimatePresence, motion } from "framer-motion";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";

type IGrowingModalProps = {
  children: React.ReactElement;
  maxWidth: number;
  maxHeight: number;
  minWidth: number;
  minHeight: number;
};

export interface GrowingModalRef {
  closeModal: () => void;
  modalActive: boolean;
}

const GrowingModal: React.FC<IGrowingModalProps> = ({
  maxWidth,
  maxHeight,
  minHeight,
  minWidth,
  children
}): JSX.Element => {
  const [_, forceUpdate] = useState(0);

  const divRef = useRef<HTMLDivElement>(null);
  const { screenSize } = useAppGeneralStateContext();

  const [animationInProgress, setAnimationInProgress] = useState(false);

  const [modalActive, setModalActive] = useState(false);

  const pageSize = useMemo(() => {
    return {
      width:
        screenSize.width > screensEnum.lg
          ? screenSize.width + 73
          : screenSize.width,
      height: screenSize.height
    };
  }, [screenSize]);

  const childrenSize = useMemo(() => {
    return {
      width: divRef.current?.offsetWidth ?? 0,
      height: divRef.current?.offsetHeight ?? 0
    };
  }, [divRef.current]);

  const childrenPos = useMemo(() => {
    return {
      x: divRef.current?.getBoundingClientRect().left ?? 0,
      y: divRef.current?.getBoundingClientRect().top ?? 0
    };
  }, [divRef.current, screenSize]);

  const modalSize = useMemo(() => {
    return {
      width: Math.min(maxWidth, pageSize.width - 100),
      height: Math.min(maxHeight, pageSize.height - 100)
    };
  }, [maxHeight, maxWidth, pageSize]);

  const modalPos = useMemo(() => {
    return {
      x: (pageSize.width - modalSize.width) / 2 - childrenPos.x,
      y: (pageSize.height - modalSize.height) / 2 - childrenPos.y
    };
  }, [screenSize, modalSize, childrenPos]);

  useEffect(() => {
    forceUpdate((prev) => prev + 1);
  }, []);

  const closeModal = () => {
    setModalActive(false);
    setAnimationInProgress(true);
  };
  return (
    <>
      <AnimatePresence>
        {modalActive ? (
          <motion.div
            className="absolute top-0 left-0 w-[100vw] h-[100vh] z-10 backdrop-blur-lg "
            transition={{ duration: 0.5, ease: "easeInOut" }}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={() => closeModal()}
          />
        ) : null}
      </AnimatePresence>
      <div className="relative flex-grow">
        <div
          style={{
            minWidth: minWidth,
            minHeight: minHeight
          }}
          className="flex-grow"
          ref={divRef}
        ></div>
        <motion.div
          style={{
            width: divRef.current?.offsetWidth,
            height: childrenSize.height
          }}
          className={`absolute  top-0 left-0 ${
            modalActive || animationInProgress ? "z-20" : ""
          }`}
          transition={{
            duration: animationInProgress ? 0.5 : 0,
            ease: "easeInOut"
          }}
          animate={
            modalActive
              ? {
                  width: modalSize.width,
                  height: modalSize.height,
                  x: modalPos.x,
                  y: modalPos.y
                }
              : {
                  width: divRef.current?.offsetWidth
                }
          }
          onClick={() => {
            if (!modalActive) {
              setModalActive(true);
              setAnimationInProgress(true);
            }
          }}
          onAnimationComplete={(e) => {
            console.log("animation finished");
            if (!modalActive) {
              setAnimationInProgress(false);
            }
          }}
        >
          {children}
          {/* <motion.button
            key="modalButton"
            className={`absolute right-0 top-0 bg-rose-800 w-[30px] h-[30px] rounded-full mt-[10px] mr-[10px] hover:bg-rose-200 z-10 ${
              modalActive ? "" : "disabled"
            }`}
            initial={{ opacity: 0 }}
            transition={{ duration: 1 }}
            animate={modalActive ? { opacity: 1 } : { opacity: 0 }}
            exit={{
              opacity: 0,
              transition: { duration: 1 }
            }}
            onClick={() => closeModal()}
          >
            X
          </motion.button> */}
        </motion.div>
      </div>
    </>
  );
};

export default GrowingModal;
