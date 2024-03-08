import React, { useRef, useState } from "react";
import CaloriesGraph from "~/features/health/components/calories/CaloriesGraph";

import { Card } from "~/chadcn/ui/card";
import FoodDiary from "./foodDiary/FoodDiary";
import FoodSearch, { SearchWithModalRef } from "./foodSearch/FoodSearch";

const CaloriesComponent: React.FC = (): JSX.Element => {
  const searchRef = useRef<SearchWithModalRef>(null);

  const [date, setDate] = useState<Date>(new Date());

  return (
    // <GrowingModal
    //   maxWidth={1000}
    //   maxHeight={1000}
    //   minWidth={322}
    //   minHeight={228}
    // >
    // <BoxStyledComponent
    //   title="Calories"
    //   className=" flex-grow overflow-hidden " //w-[calc(100%-33px)]
    //   onClick={() => {
    //     searchRef.current?.setResultsTableOpened(false);
    //   }}
    //   // scrollable={screenSize.height < 1000}
    // >
    <Card className="h-full w-full">
      <CaloriesGraph />
      {/* <div className="flex h-[calc(100%-195px)] w-full flex-col items-center"> */}

      <div className="flex  w-full flex-col items-center">
        {/* <div className="w-[90%] lg:w-[80%] "> */}
        <div className="w-[90%]">
          <FoodSearch ref={searchRef} date={date} />
          <FoodDiary date={date} setDate={setDate} />
        </div>
      </div>
    </Card>
    // </BoxStyledComponent>
    // </GrowingModal>
  );
};

export default CaloriesComponent;
