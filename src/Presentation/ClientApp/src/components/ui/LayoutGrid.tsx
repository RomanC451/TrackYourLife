import { useState } from "react";
import Content from "./Content";
import SelectedContent from "./SelectedContent";

export default function LayoutGrid() {
  const projectObjArray = [
    {
      title: "Project 1",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 2",
      description:
        "some descriptionsssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 3",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 4",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 5",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 6",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 7",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 8",
      description: "some description",
      background: "#f00",
      link: "",
    },
    {
      title: "Project 9",
      description: "some description",
      background: "#f00",
      link: "",
    },
  ];

  const [expanded, setExpanded] = useState<number | undefined>(undefined);

  return (
    <div>
      <h1 className="mb-5 pb-5 text-6xl md:ml-10 md:text-left">My Projects</h1>
      <div className="grid grid-cols-1 items-center gap-10 md:grid-cols-3">
        {projectObjArray.map((el, index) => {
          if (index !== expanded) {
            return (
              <Content
                projectobj={el}
                key={el.title}
                index={index}
                setexpanded={setExpanded}
              />
            );
          } else {
            return (
              <SelectedContent
                projectobj={el}
                key={el.title}
                index={index}
                setexpanded={setExpanded}
              />
            );
          }
        })}
      </div>
    </div>
  );
}
