"use no memo";

import { ColumnDef } from "@tanstack/react-table";
import { ArrowDown, ArrowUp, ArrowUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import Spinner from "@/components/ui/spinner";
import { RecipeDto } from "@/services/openapi";

import RecipesTableRowActionsMenu from "./RecipesTableRowActionsMenu";

// eslint-disable-next-line react-refresh/only-export-components
function SortIcon({ isSorted }: { isSorted: false | "asc" | "desc" }) {
  if (isSorted === false) return <ArrowUpDown />;
  if (isSorted === "asc") return <ArrowUp className="text-primary" />;
  if (isSorted === "desc") return <ArrowDown className="text-primary" />;
}

export const recipesTableColumns: ColumnDef<RecipeDto>[] = [
  {
    id: "select",
    header: ({ table }) => (
      <Checkbox
        checked={
          table.getIsAllPageRowsSelected() ||
          (table.getIsSomePageRowsSelected() && "indeterminate")
        }
        onCheckedChange={(value) => table.toggleAllPageRowsSelected(!!value)}
        aria-label="Select all"
        className="ml-2"
      />
    ),
    cell: ({ row }) =>
      row.original.isDeleting ? (
        <Spinner />
      ) : (
        <Checkbox
          checked={row.getIsSelected()}
          onCheckedChange={(value) => row.toggleSelected(!!value)}
          aria-label="Select row"
          disabled={row.original.isDeleting}
        />
      ),
    enableSorting: false,
    enableHiding: false,
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const recipe = row.original;

      return <RecipesTableRowActionsMenu recipe={recipe} />;
    },

    header: () => (
      <Button variant="ghost" className="w-8" disabled={true}>
        <div className="h-4 w-4" />
      </Button>
    ),
  },
  {
    accessorKey: "name",
    header: ({ column }) => {
      return (
        <Button
          className="p-2"
          variant="ghost"
          onClick={() => column.toggleSorting()}
        >
          Name
          <SortIcon isSorted={column.getIsSorted()} />
        </Button>
      );
    },
    cell: ({ row }) => <p>{row.original.name}</p>,
    enableSorting: true,
  },
  {
    accessorKey: "portions",
    header: () => <p className="p-2">Portions</p>,
    cell: ({ row }) => row.original.portions,
  },
  {
    accessorKey: "calories",
    header: ({ column }) => {
      return (
        <Button
          className="p-2"
          variant="ghost"
          onClick={() => column.toggleSorting()}
        >
          Calories
          <SortIcon isSorted={column.getIsSorted()} />
        </Button>
      );
    },
    cell: ({ row }) => row.original.nutritionalContents.energy.value.toFixed(1),
    accessorFn: (row) => row.nutritionalContents.energy.value,
  },
  {
    accessorKey: "carbs",
    header: ({ column }) => {
      return (
        <Button
          className="p-2"
          variant="ghost"
          onClick={() => column.toggleSorting()}
        >
          Carbs
          <SortIcon isSorted={column.getIsSorted()} />
        </Button>
      );
    },
    cell: ({ row }) =>
      row.original.nutritionalContents.carbohydrates.toFixed(1),
    accessorFn: (row) => row.nutritionalContents.carbohydrates,
  },
  {
    accessorKey: "fat",
    header: ({ column }) => {
      return (
        <Button
          className="p-2"
          variant="ghost"
          onClick={() => column.toggleSorting()}
        >
          Fat
          <SortIcon isSorted={column.getIsSorted()} />
        </Button>
      );
    },
    cell: ({ row }) => row.original.nutritionalContents.fat.toFixed(1),
    accessorFn: (row) => row.nutritionalContents.fat,
  },
  {
    accessorKey: "protein",
    header: ({ column }) => {
      return (
        <Button
          className="p-2"
          variant="ghost"
          onClick={() => column.toggleSorting()}
        >
          Protein
          <SortIcon isSorted={column.getIsSorted()} />
        </Button>
      );
    },
    cell: ({ row }) => row.original.nutritionalContents.protein.toFixed(1),
    accessorFn: (row) => row.nutritionalContents.protein,
  },
];
