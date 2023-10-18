import { ArcElement, Chart as ChartJS, Legend, Tooltip } from "chart.js";
import React from "react";
import { Doughnut } from "react-chartjs-2";

ChartJS.register(ArcElement, Tooltip, Legend);

const graphOtions = {
  cutout: "90%",
  plugins: {
    legend: {
      display: false
    }
  },
  responsive: true,
  maintainAspectRatio: true
};

interface IProps {
  radius: number;
  colors: string[];
  userData: number[];
  labels: string[];
}

const DoughnutChart: React.FC<IProps> = ({
  radius,
  colors,
  userData,
  labels
}): JSX.Element => {
  const graphUserData = userData.flatMap((value, index, array) =>
    index === array.length ? [value] : [value, 1]
  );

  const graphLabels = labels.flatMap((value, index, array) =>
    index === array.length ? [value] : [value, ""]
  );

  const graphColors = colors.flatMap((value, index, array) =>
    index === array.length ? [value] : [value, "transparent"]
  );

  const graphData = {
    labels: graphLabels,
    datasets: [
      {
        label: "My First Dataset",
        data: graphUserData,
        backgroundColor: graphColors,
        rotation: -150,
        hoverOffset: 4,
        borderRadius: 50,
        borderWidth: 5,
        borderColor: "transparent"
      }
    ]
  };

  return (
    <div style={{ width: radius, height: radius }}>
      <Doughnut data={graphData} options={graphOtions} />
    </div>
  );
};

export default DoughnutChart;
