import "./testPage.css";

import { AnimatePresence, motion } from "framer-motion";
import { wrap } from "popmotion";
import { useState } from "react";

const items = [
  { id: "0", subtitle: "subtitle 0", title: "title 0 " },
  { id: "1", subtitle: "subtitle 1", title: "title 1 " },
  { id: "2", subtitle: "subtitle 2", title: "title 2 " }
];

const TestPage = () => {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const item = items.find((i) => i.id === selectedId);

  return (
    <>
      {items.map((item) => (
        <motion.div
          key={item.id}
          layoutId={item.id}
          onClick={() => setSelectedId(item.id)}
        >
          <motion.h5>{item.subtitle}</motion.h5>
          <motion.h2>{item.title}</motion.h2>
        </motion.div>
      ))}

      <AnimatePresence>
        {selectedId && (
          <motion.div layoutId={selectedId}>
            <motion.h5>{item?.subtitle}</motion.h5>
            <motion.h2>{item?.title}</motion.h2>
            <motion.button onClick={() => setSelectedId(null)} />
          </motion.div>
        )}
      </AnimatePresence>
    </>
  );
};
export default TestPage;
