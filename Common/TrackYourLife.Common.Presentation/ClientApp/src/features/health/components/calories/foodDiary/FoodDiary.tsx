import React from "react";

import { CardTitle } from "~/chadcn/ui/card";
import { ScrollArea, ScrollBar } from "~/chadcn/ui/scroll-area";
import useFoodDiary from "../../../hooks/useFoodDiary";
import DataTable from "./diaryTable/DataTable";
import { DataTableViewOptions } from "./diaryTable/DataTableViewOptions";
import DatePicker from "./diaryTable/DatePicker";

type FoodDiaryProps = {
  date: Date;
  setDate: (date: Date) => void;
};

const FoodDiary: React.FC<FoodDiaryProps> = ({ date, setDate }) => {
  const { tables, loadingState } = useFoodDiary(date);
  return (
    <div className="mt-3 w-full ">
      <div className="mb-2 flex w-full justify-between">
        <CardTitle className="mb-3 text-left">FoodDiary</CardTitle>
        <div className="flex flex-wrap justify-end gap-2 sm:flex-nowrap">
          <DatePicker date={date} setDate={setDate} />
          <DataTableViewOptions table={tables.breakfastTable} />
        </div>
      </div>

      <ScrollArea className="rounded-lg border-[1px]">
        <DataTable tables={tables} loadingState={loadingState} />
        <ScrollBar orientation="horizontal" className="mb-[-1px]" />
      </ScrollArea>
    </div>
  );
};

export default FoodDiary;
