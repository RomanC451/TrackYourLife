import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "~/chadcn/ui/table";

import { flexRender, Table as TTable } from "@tanstack/react-table";

import { CircularProgress } from "@mui/material";
import { Card, CardTitle } from "~/chadcn/ui/card";
import { ScrollArea, ScrollBar } from "~/chadcn/ui/scroll-area";
import { Skeleton } from "~/chadcn/ui/skeleton";
import useFoodDiaryTables from "~/features/health/hooks/foodDiaries/useFoodDiaryTables";
import { LoadingState } from "~/hooks/useDelayedLoading";
import { NutritionDiaryDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import DatePicker from "./DatePicker";
import { foodDiaryTableColumns } from "./foodDiaryTableColumns";
import { TableViewOptions } from "./TableViewOptions";

interface FoodDiaryTableProps {
  date: DateOnly;
  setDate: (date: Date) => void;
}

export function FoodDiaryTable({ date, setDate }: FoodDiaryTableProps) {
  const { tables, loadingState } = useFoodDiaryTables(date);

  return (
    <div className="mt-3 w-full ">
      <FoodDiaryTable.Header
        date={date}
        setDate={setDate}
        table={tables.breakfastTable}
      />
      <ScrollArea className="rounded-lg border-[1px]">
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              {tables.breakfastTable.getHeaderGroups().map((headerGroup) => (
                <TableRow key={headerGroup.id}>
                  {headerGroup.headers.map((header) => {
                    return (
                      <TableHead className="p-0" key={header.id}>
                        {header.isPlaceholder
                          ? null
                          : flexRender(
                              header.column.columnDef.header,
                              header.getContext(),
                            )}
                      </TableHead>
                    );
                  })}
                </TableRow>
              ))}
            </TableHeader>
            <TableBody>
              <FoodDiaryTable.Body
                table={tables.breakfastTable}
                name="Breakfast"
                loadingState={loadingState}
              />
              <FoodDiaryTable.Body
                table={tables.lunchTable}
                name="Lunch"
                loadingState={loadingState}
              />

              <FoodDiaryTable.Body
                table={tables.dinnerTable}
                name="Dinner"
                loadingState={loadingState}
              />

              <FoodDiaryTable.Body
                table={tables.snacksTable}
                name="Snacks"
                loadingState={loadingState}
              />
            </TableBody>
          </Table>
        </div>
        <ScrollBar orientation="horizontal" className="mb-[-1px]" />
      </ScrollArea>
    </div>
  );
}

FoodDiaryTable.Loading = function () {
  return (
    <Card className="flex flex-grow">
      <div className="flex flex-grow items-center justify-center">
        <CircularProgress />
      </div>
    </Card>
  );
};

FoodDiaryTable.Header = function ({
  date,
  setDate,
  table,
}: {
  date: DateOnly;
  setDate: (date: Date) => void;
  table: TTable<NutritionDiaryDto>;
}) {
  return (
    <div className="mb-2 flex w-full flex-col flex-wrap justify-between sm:flex-row sm:flex-nowrap">
      <CardTitle className="mb-3 text-left">FoodDiary</CardTitle>
      <div className="flex flex-wrap justify-between gap-2  sm:flex-nowrap sm:justify-end">
        <DatePicker dateOnly={date} setDate={setDate} />
        <TableViewOptions table={table} />
      </div>
    </div>
  );
};

FoodDiaryTable.Body = function ({
  table,
  name,
  loadingState,
}: {
  table: TTable<NutritionDiaryDto>;
  name: string;
  loadingState: LoadingState;
}) {
  const totalValues = table.getRowModel().rows.reduce(
    (totals, row) => {
      const { energy, carbohydrates, fat, protein } =
        row.original.nutritionalContents;

      return {
        calories: totals.calories + energy.value,
        carbs: totals.carbs + carbohydrates,
        fat: totals.fat + fat,
        protein: totals.protein + protein,
      };
    },
    { calories: 0, carbs: 0, fat: 0, protein: 0 },
  );

  return (
    <>
      <TableRow className="border-b-1 h-12 border-b-violet-500/70">
        <TableCell
          colSpan={3}
          className="h-12 bg-accent/50 py-2 text-left font-bold"
        >
          {name}
        </TableCell>
        {table.getVisibleFlatColumns().map((column, index) => {
          if (column.id && column.id in totalValues)
            return (
              <TableCell
                key={`total-cell-${index}`}
                className="h-12 bg-accent/50 py-2 text-left font-bold"
              >
                {totalValues[column.id as keyof typeof totalValues].toFixed(1)}
              </TableCell>
            );
        })}
      </TableRow>

      {loadingState.isStarting ? (
        <TableRow>
          <TableCell
            colSpan={foodDiaryTableColumns.length}
            className="h-12 py-2 text-left"
          />
        </TableRow>
      ) : loadingState.isLoading ? (
        <TableRow>
          {table.getVisibleFlatColumns().map((column, index) => {
            if (index == 0)
              return (
                <TableCell key={column.id} className="h-12 py-2 text-left">
                  <Skeleton className="mx-2 h-[16px] w-[16px]" />
                </TableCell>
              );
            return (
              <TableCell key={column.id} className="h-12 py-2 text-left">
                <Skeleton className="h-[20px] w-[75px]" />
              </TableCell>
            );
          })}
        </TableRow>
      ) : table.getRowModel().rows?.length ? (
        table.getRowModel().rows.map((row) => (
          <TableRow
            key={row.id}
            data-state={row.getIsSelected() && "selected"}
            className="h-12"
          >
            {row.getVisibleCells().map((cell) => (
              <TableCell key={cell.id} className="h-12 py-2">
                {flexRender(cell.column.columnDef.cell, cell.getContext())}
              </TableCell>
            ))}
          </TableRow>
        ))
      ) : (
        <TableRow>
          <TableCell
            colSpan={foodDiaryTableColumns.length}
            className="h-12 text-left"
          />
        </TableRow>
      )}
    </>
  );
};

export default FoodDiaryTable;
