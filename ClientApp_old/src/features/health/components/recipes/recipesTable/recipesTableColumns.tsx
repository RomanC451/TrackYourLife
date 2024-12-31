import { ColumnDef } from "@tanstack/react-table";
import { ArrowUpDown } from "lucide-react";
import { Button } from "~/chadcn/ui/button";
import { RecipeDto } from "~/services/openapi";
import RecipesTableRowActionsMenu from "./RecipesTableRowActionsMenu";

export const recipesTableColumns: ColumnDef<RecipeDto>[] = [
  {
    id: "actions",
    cell: ({ row }) => {
      const recipe = row.original;

      return <RecipesTableRowActionsMenu recipe={recipe} />;
    },

    header: () => (
      <Button variant="ghost" className="w-10" disabled>
        <div className="h-4 w-4" />
      </Button>
    ),
  },
  {
    accessorKey: "name",
    header: ({ column }) => {
      return (
        <Button
          className=""
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
    cell: ({ row }) => <p>{row.original.name}</p>,
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
    cell: ({ row }) => row.original.nutritionalContents.energy.value.toFixed(1),
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
    cell: ({ row }) =>
      row.original.nutritionalContents.carbohydrates.toFixed(1),
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
    cell: ({ row }) => row.original.nutritionalContents.fat.toFixed(1),
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
    cell: ({ row }) => row.original.nutritionalContents.protein.toFixed(1),
  },
];
