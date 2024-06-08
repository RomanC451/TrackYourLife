import React from "react";
import { Skeleton } from "~/chadcn/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "~/chadcn/ui/table";
import { TLoadingState } from "~/hooks/useDelayedLoading";

import { flexRender, Table as TTable } from "@tanstack/react-table";

import { columns } from "../diaryTable/columns";
import { FoodDiaryEntry } from "./columns";

interface DataTableProps {
  tables: {
    breakfastTable: TTable<FoodDiaryEntry>;
    lunchTable: TTable<FoodDiaryEntry>;
    dinnerTable: TTable<FoodDiaryEntry>;
    snacksTable: TTable<FoodDiaryEntry>;
  };
  loadingState: TLoadingState;
}

export const DataTable: React.FC<DataTableProps> = ({
  tables,
  loadingState,
}) => {
  function renderTable(table: TTable<FoodDiaryEntry>, name: string) {
    return (
      <>
        <TableRow className="border-b-1 h-12 border-b-violet-500/70">
          <TableCell
            colSpan={columns.length}
            className="h-12 bg-accent/50 py-2 text-left font-bold"
          >
            {name}
          </TableCell>
        </TableRow>

        {loadingState.isStarting ? (
          <TableRow>
            <TableCell
              colSpan={columns.length}
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
            <TableCell colSpan={columns.length} className="h-12 text-left" />
          </TableRow>
        )}
      </>
    );
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          {tables.breakfastTable.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <TableHead key={header.id}>
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
          {renderTable(tables.breakfastTable, "Breakfast")}
          {renderTable(tables.lunchTable, "Lunch")}
          {renderTable(tables.dinnerTable, "Dinner")}
          {renderTable(tables.snacksTable, "Snacks")}
        </TableBody>
      </Table>
    </div>
  );
};

export default DataTable;
