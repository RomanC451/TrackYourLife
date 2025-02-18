import { ColumnDef } from "@tanstack/react-table";
import { ArrowUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  HybridTooltip,
  HybridTooltipContent,
  HybridTooltipTrigger,
} from "@/components/ui/hybrid-tooltip";
import { DiaryType, NutritionDiaryDto } from "@/services/openapi/api";

import FoodDiaryTableRowActionsMenu from "./FoodDiaryTableRowActionsMenu";

export type FoodDiaryTableData = {
  Breakfast: NutritionDiaryDto[];
  Lunch: NutritionDiaryDto[];
  Dinner: NutritionDiaryDto[];
  Snacks: NutritionDiaryDto[];
};

export const foodDiaryTableColumns: ColumnDef<NutritionDiaryDto>[] = [
  {
    id: "actions",
    cell: ({ row }) => {
      const diary = row.original;

      return <FoodDiaryTableRowActionsMenu diary={diary} />;
    },

    header: () => (
      <Button variant="ghost" className="p" disabled>
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
    cell: ({ row }) => (
      <HybridTooltip>
        <HybridTooltipTrigger>
          <div className="inline-flex items-center gap-2">
            <p>
              {row.original.diaryType === DiaryType.FoodDiary ? "(F)" : "(R)"}
            </p>
            <p>{row.original.name}</p>
          </div>
        </HybridTooltipTrigger>
        <HybridTooltipContent className="w-full">
          {row.original.diaryType === DiaryType.FoodDiary
            ? "Food entry"
            : "Recipe entry"}
        </HybridTooltipContent>
      </HybridTooltip>
    ),
    enableHiding: false,
  },
  {
    accessorKey: "quantity",
    header: () => <p>Quantity</p>,
    cell: ({ row }) => {
      const item = row.original;
      return item.servingSize
        ? `${item.quantity * item.servingSize!.value} ${item.servingSize!.unit}`
        : `${item.quantity} portions`;
    },
    enableHiding: false,
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
    cell: ({ row }) => {
      return parseFloat(
        (
          row.original.quantity * row.original.nutritionalContents.energy.value
        ).toFixed(1),
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
    cell: ({ row }) => {
      return parseFloat(
        (
          row.original.quantity * row.original.nutritionalContents.carbohydrates
        ).toFixed(1),
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
    cell: ({ row }) => {
      return parseFloat(
        (
          row.original.quantity * row.original.nutritionalContents.protein
        ).toFixed(1),
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
    cell: ({ row }) => {
      return parseFloat(
        (row.original.quantity * row.original.nutritionalContents.fat).toFixed(
          1,
        ),
      );
    },
  },
];
