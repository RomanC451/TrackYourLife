import { ArrowUpDown } from "lucide-react";
import { Button } from "~/chadcn/ui/button";
import { TMealTypes } from "~/features/health/data/enums";

import { DateOnly } from "~/utils/date";

import { ColumnDef } from "@tanstack/react-table";

import { FoodResponse, ServingSize } from "~/services/openapi/api";
import RowActionsMenu from "./RowActionsMenu";

export type FoodDiaryEntry = {
  name: string;
  brandName: string;
  calories: number;
  quantity: string;
  carbs: number;
  fat: number;
  protein: number;
  meal: TMealTypes;
  id: string;
  foodId: string;
  servingSize: ServingSize;
  nrOfServings: number;
  date: DateOnly;
  food: FoodResponse;
};

export const columns: ColumnDef<FoodDiaryEntry>[] = [
  {
    id: "actions",
    cell: ({ row }) => {
      const diaryEntry = row.original;

      return <RowActionsMenu diaryEntry={diaryEntry} />;
    },

    header: () => (
      <Button variant="ghost" className="w-16" disabled>
        <div className="h-4 w-4" />
      </Button>
    ),
  },
  {
    accessorKey: "name",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "brandName",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Brand Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "quantity",
    header: () => <p>Quantity</p>,
  },
  {
    accessorKey: "calories",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Calories
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "carbs",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Carbs
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "fat",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Fat
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "protein",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Protein
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
];
