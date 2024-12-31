import { motion } from "framer-motion";
import { useRef } from "react";
type ContentProps = {
  setexpanded: (index: number | undefined) => void;
  projectobj: {
    title: string;
    description: string;
    background: string;
    link: string;
  };
  index: number;
};

export default function SelectedContent(props: ContentProps) {
  const difRef = useRef<HTMLDivElement>(null);
  return (
    <>
      <div className="h-56 w-full" ref={difRef}></div>
      <motion.div
        className="fixed   w-full flex-col items-center justify-center"
        style={{ background: "rgba(0, 0, 0, 0.5)" }}
        onClick={() => {
          props.setexpanded(undefined);
        }}
      >
        <motion.div
          layoutId={`Container${props.projectobj.title}`}
          transition={{ duration: 20 }}
          initial={{ borderRadius: "50px" }}
          animate={{ borderRadius: "100px" }}
          style={{
            background: props.projectobj.background,
            width: "80vw",
            height: "50vh",
          }}
          onClick={(e) => {
            e.stopPropagation();
          }}
          className="flex flex-col items-center justify-center"
        >
          <motion.div
            className="bg-green-500"
            layoutId={`Title${props.projectobj.title}`}
            layout
            transition={{ duration: 2 }}
          >
            <h1
              // layoutId={`Title${props.projectobj.title}`}
              // // layout
              // transition={{ duration: 2 }}
              className="text-2xl"
            >
              {props.projectobj.title}
            </h1>
            <p
              // layoutId={`Description${props.projectobj.title}`}
              // // layout
              // transition={{ duration: 2 }}
              className="text-1xl"
            >
              {props.projectobj.description}
            </p>
            <div>
              <div>
                <span>asdsa</span>
              </div>
            </div>
          </motion.div>
        </motion.div>
      </motion.div>
    </>
  );
}
