import { motion } from "framer-motion";

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

export default function Content(props: ContentProps) {
  return (
    <motion.div
      //   initial={{ borderRadius: "100px" }}
      //   animate={{ borderRadius: "50px" }}
      className=" h-56 w-full "
      style={{ background: props.projectobj.background }}
      onClick={() => {
        props.setexpanded(props.index);
      }}
      //   layoutId={`Container${props.projectobj.title}`}
      //   transition={{ duration: 20 }}
    >
      <motion.div
        className=" h-full w-full bg-green-500"
        layoutId={`Title${props.projectobj.title}`}
        layout
        transition={{ duration: 20 }}
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
  );
}
