import { flexRender, Table as TTable } from "@tanstack/react-table";

import { Card, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { DateOnly } from "@/lib/date";
import { NutritionDiaryDto } from "@/services/openapi";

import DatePicker from "./DatePicker";
import { foodDiaryTableColumns } from "./foodDiaryTableColumns";
import { TableViewOptions } from "./TableViewOptions";
import useFoodDiaryTables from "./useFoodDiaryTables";

import "./scrollbar.css";

import { v4 as uuidv4 } from "uuid";

import Spinner from "@/components/ui/spinner";
import { PendingState } from "@/hooks/useCustomQuery";
import { cn } from "@/lib/utils";

interface FoodDiaryTableProps {
  date: DateOnly;
  setDate: (date: Date) => void;
}

function calculateTotalValues(table: TTable<NutritionDiaryDto>) {
  return table.getRowModel().rows.reduce(
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
}

export function FoodDiaryTable({ date, setDate }: FoodDiaryTableProps) {
  const { tables, pendingState } = useFoodDiaryTables(date);

  const breakfastTotalValues = calculateTotalValues(tables.breakfastTable);
  const lunchTotalValues = calculateTotalValues(tables.lunchTable);
  const dinnerTotalValues = calculateTotalValues(tables.dinnerTable);
  const snacksTotalValues = calculateTotalValues(tables.snacksTable);

  const totalValues = {
    calories:
      breakfastTotalValues.calories +
      lunchTotalValues.calories +
      dinnerTotalValues.calories +
      snacksTotalValues.calories,
    carbs:
      breakfastTotalValues.carbs +
      lunchTotalValues.carbs +
      dinnerTotalValues.carbs +
      snacksTotalValues.carbs,
    fat:
      breakfastTotalValues.fat +
      lunchTotalValues.fat +
      dinnerTotalValues.fat +
      snacksTotalValues.fat,
    protein:
      breakfastTotalValues.protein +
      lunchTotalValues.protein +
      dinnerTotalValues.protein +
      snacksTotalValues.protein,
  };

  return (
    <Card className="p-4">
      <FoodDiaryTable.Header
        date={date}
        setDate={setDate}
        table={tables.breakfastTable}
      />
      {/* <Separator className="mb-2 w-full" /> */}
      <div className="custom-scrollbar scrollbar-thumb-sky-700 scrollbar-track-sky-300 relative max-h-[calc(100vh-310px)] overflow-auto rounded-md [@media(min-height:1250px)]:max-h-[calc(100vh-600px)]">
        <Table>
          <TableHeader className="sticky top-0 bg-card-secondary">
            {tables.breakfastTable.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead
                      className="p-0"
                      key={header.id}
                      style={{
                        minWidth: header.column.columnDef.minSize,
                      }}
                    >
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
              pendingState={pendingState}
            />
            <FoodDiaryTable.Body
              table={tables.lunchTable}
              name="Lunch"
              pendingState={pendingState}
            />

            <FoodDiaryTable.Body
              table={tables.dinnerTable}
              name="Dinner"
              pendingState={pendingState}
            />

            <FoodDiaryTable.Body
              table={tables.snacksTable}
              name="Snacks"
              pendingState={pendingState}
            />
            <TableRow className="">
              <TableCell
                colSpan={3}
                className="h-12 bg-accent/50 py-2 text-left font-bold"
              >
                Total
              </TableCell>
              {tables.snacksTable.getVisibleFlatColumns().map((column) => {
                if (
                  column.id &&
                  ["calories", "carbs", "fat", "protein"].includes(column.id)
                )
                  return (
                    <TableCell
                      key={`total-cell-${uuidv4()}`}
                      className="h-12 bg-accent/50 px-4 py-2 text-left font-bold"
                    >
                      {totalValues[
                        column.id as keyof typeof totalValues
                      ].toFixed()}
                    </TableCell>
                  );
              })}
            </TableRow>
          </TableBody>
        </Table>
      </div>
    </Card>
  );
}

FoodDiaryTable.Loading = function () {
  return (
    <Card className="flex flex-grow">
      <div className="flex flex-grow items-center justify-center">
        <Spinner />
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
    <div className="mb-2 flex w-full flex-col flex-wrap items-center justify-between gap-4 sm:flex-row sm:flex-nowrap">
      <CardTitle className="text-nowrap text-left">Food history</CardTitle>
      <div className="flex flex-wrap justify-between gap-2 sm:flex-nowrap sm:justify-end">
        <DatePicker dateOnly={date} setDate={setDate} />
        <TableViewOptions table={table} />
      </div>
    </div>
  );
};

FoodDiaryTable.Body = function ({
  table,
  name,
  pendingState,
}: {
  table: TTable<NutritionDiaryDto>;
  name: string;
  pendingState: PendingState;
}) {
  const totalValues = calculateTotalValues(table);

  const renderTableContent = () => {
    if (pendingState.isDelayedPending) {
      return (
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
      );
    }

    if (pendingState.isPending) {
      return (
        <TableRow>
          <TableCell
            colSpan={foodDiaryTableColumns.length}
            className="h-12 py-2 text-left"
          />
        </TableRow>
      );
    }

    if (table.getRowModel().rows?.length) {
      return table.getRowModel().rows.map((row) => (
        <TableRow
          key={row.id}
          className={cn("h-12", { "opacity-50": row.original.isDeleting })}
        >
          {row.getVisibleCells().map((cell) => (
            <TableCell key={cell.id} className="h-12 py-2">
              {flexRender(cell.column.columnDef.cell, cell.getContext())}
            </TableCell>
          ))}
        </TableRow>
      ));
    }

    return (
      <TableRow>
        <TableCell
          colSpan={foodDiaryTableColumns.length}
          className="h-12 text-left"
        />
      </TableRow>
    );
  };

  return (
    <>
      <TableRow className="border-b-1 h-12 border-b-violet-500/70">
        <TableCell
          colSpan={3}
          className="h-12 bg-card-secondary/50 py-2 text-left font-bold"
        >
          {name}
        </TableCell>
        {table.getVisibleFlatColumns().map((column) => {
          if (column.id && column.id in totalValues)
            return (
              <TableCell
                key={`total-cell-${name}-${uuidv4()}`}
                className="h-12 bg-card-secondary/50 px-4 py-2 text-left font-bold"
              >
                {totalValues[column.id as keyof typeof totalValues].toFixed()}
              </TableCell>
            );
        })}
      </TableRow>
      {renderTableContent()}
    </>
  );
};

export default FoodDiaryTable;
