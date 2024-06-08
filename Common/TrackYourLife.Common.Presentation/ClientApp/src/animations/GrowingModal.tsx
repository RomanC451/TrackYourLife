import { AnimatePresence, motion, useAnimationControls } from "framer-motion";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { Button } from "~/chadcn/ui/button";
import { screensEnum } from "~/constants/tailwindSizes";
import { useAppGeneralStateContext } from "~/contexts/AppGeneralContextProvider";
import { useSideBarContext } from "~/contexts/SideBarContextProvider";

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
  children,
}): JSX.Element => {
  const { screenSize } = useAppGeneralStateContext();

  const { sideBarWidth } = useSideBarContext();

  const [, forceUpdate] = useState(0);

  const [animationInProgress, setAnimationInProgress] = useState(false);

  const [modalActive, setModalActive] = useState(false);

  const divRef = useRef<HTMLDivElement>(null);

  const pageSize = useMemo(() => {
    return {
      width:
        screenSize.width > screensEnum.lg
          ? screenSize.width - sideBarWidth
          : screenSize.width,
      height: screenSize.height,
    };
  }, [screenSize, sideBarWidth]);

  const getChildrenPos = () => {
    return {
      x: divRef.current?.getBoundingClientRect().x ?? 0,
      y: divRef.current?.getBoundingClientRect().y ?? 0,
    };
  };

  const getModalSize = () => {
    return {
      width: Math.min(maxWidth, pageSize.width - 50),
      height: maxHeight, //Math.min(maxHeight, pageSize.height - 50)
    };
  };

  const getModalPos = () => {
    const modalSize = getModalSize();
    const childrenPos = getChildrenPos();
    return {
      x: (pageSize.width - modalSize.width) / 2 - childrenPos.x + sideBarWidth,
      y:
        modalSize.height > screenSize.height
          ? 50
          : (pageSize.height - modalSize.height) / 2 - childrenPos.y,
    };
  };

  useEffect(() => {
    forceUpdate((prev) => prev + 1);
  }, []);

  useEffect(() => {
    if (!modalActive) return;
    const modalSize = getModalSize();
    const modalPos = getModalPos();

    animation.set({
      width: modalSize.width,
      height: modalSize.height,
      x: modalPos.x,
      y: modalPos.y,
    });
  }, [screenSize, sideBarWidth]);

  useEffect(() => {
    const closeModalOnPressedEsc = (event: KeyboardEvent) => {
      if (event.defaultPrevented) {
        return; // Do nothing if the event was already processed
      }
      if (event.key !== "Escape" || modalActive === false) {
        return;
      }

      closeModal();
    };

    window.addEventListener("keydown", closeModalOnPressedEsc);

    return () => window.removeEventListener("keydown", closeModalOnPressedEsc);
  }, [modalActive]);

  const animation = useAnimationControls();

  const closeModal = () => {
    if (animationInProgress) return;
    setAnimationInProgress(true);
    setModalActive(false);
    // enableBodyScroll(document);

    animation
      .start({
        x: 0,
        y: 0,
        width: "100%",
        height: "100%",
      })
      .then(() => {
        animation.set({ zIndex: 0 });
        setAnimationInProgress(false);
      });
  };

  const openModal = () => {
    if (animationInProgress) return;
    if (modalActive) return;
    // disableBodyScroll;
    const modalSize = getModalSize();
    const modalPos = getModalPos();

    // disableBodyScroll(document);

    animation.start({
      width: modalSize.width,
      height: modalSize.height,
      x: modalPos.x,
      y: modalPos.y,
      zIndex: 20,
      flexGrow: 0,
    });

    setModalActive(true);
  };

  return (
    <>
      <AnimatePresence>
        {modalActive ? (
          <motion.div
            style={{
              width: pageSize.width + 200,
              height: "100%",
            }}
            className="absolute top-0 left-0  z-10 backdrop-blur-lg self-auto"
            transition={{ duration: 0.5, ease: "easeInOut" }}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={() => {
              if (screenSize.width <= screensEnum.lg) return;
              closeModal();
            }}
          />
        ) : null}
      </AnimatePresence>
      <div
        className="relative flex-grow "
        style={{ minWidth: minWidth, minHeight: minHeight }}
      >
        <div ref={divRef} className=" flex-grow"></div>

        <motion.div
          className="absolute w-full top-0 left-0 h-full "
          transition={{
            duration: 0.5,
            ease: "easeInOut",
          }}
          animate={animation}
          onClick={openModal}
        >
          {children}
          <AnimatePresence>
            {screenSize.width <= screensEnum.lg ? (
              <motion.div
                key="modalButton"
                className={`absolute right-[5px] top-[35px]  mt-[10px] mr-[10px] z-10 ${
                  modalActive ? "" : "disabled"
                }`}
                initial={{ opacity: 0 }}
                transition={{ duration: 0.5 }}
                animate={modalActive ? { opacity: 1 } : { opacity: 0 }}
                exit={{
                  opacity: 0,
                }}
                onClick={() => closeModal()}
              >
                <Button variant="ghost">X</Button>
              </motion.div>
            ) : null}
          </AnimatePresence>
        </motion.div>
      </div>
    </>
  );
};

export default GrowingModal;
