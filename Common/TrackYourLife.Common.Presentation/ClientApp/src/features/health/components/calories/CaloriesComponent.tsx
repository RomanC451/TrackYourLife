import React, { useRef, useState } from "react";
import CaloriesGraph from "~/features/health/components/calories/CaloriesGraph";

import { Card } from "~/chadcn/ui/card";
import FoodDiary from "./foodDiary/FoodDiary";
import FoodSearch, { SearchWithModalRef } from "./foodSearch/FoodSearch";

const CaloriesComponent: React.FC = (): JSX.Element => {
  const searchRef = useRef<SearchWithModalRef>(null);

  const [date, setDate] = useState<Date>(new Date());

  return (
    <Card className="h-full w-full p-8">
      <CaloriesGraph />

      <div className="flex  w-full flex-col items-center">
        <div className="w-full">
          <FoodSearch ref={searchRef} date={date} />
          <FoodDiary date={date} setDate={setDate} />
        </div>
      </div>
    </Card>
  );
};

export default CaloriesComponent;
