import { cn } from "../../utils/utils";
import "./hideCorners.css";

interface IProps {
  color: string;
  width: number;
  rightOffset: number;
}

export const HideCorners: React.FC<IProps> = ({
  color,
  width,
  rightOffset,
}): JSX.Element => {
  const cornersClassNames = "absolute cover bottom-[-0.5px]";

  return (
    <>
      <div
        style={
          {
            "--corner-color": color,
            "--size": `${width}px`,
          } as React.CSSProperties
        }
        corner-color={color}
        className={cn(cornersClassNames, "-rotate-90 left-0")}
      ></div>
      <div
        style={
          {
            "--corner-color": color,
            "--size": `${width}px`,
            left: `${rightOffset}px`,
          } as React.CSSProperties
        }
        corner-color={color}
        className={cn(cornersClassNames, "-rotate-180")}
      ></div>
    </>
  );
};
