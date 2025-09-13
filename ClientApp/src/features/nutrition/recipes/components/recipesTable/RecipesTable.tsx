import { useSuspenseQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import {
  flexRender,
  RowSelectionState,
  Table as TableT,
} from "@tanstack/react-table";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Card } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";
import { RecipeDto } from "@/services/openapi";

import useDeleteRecipesMutation from "../../mutations/useDeleteRecipesMutation";
import { recipesQueryOptions } from "../../queries/useRecipeQuery";
import { recipesTableColumns } from "./recipesTableColumns";
import useRecipesTable from "./useRecipesTable";

function RecipesTable() {
  const { data } = useSuspenseQuery(recipesQueryOptions.all);

  const { table, rowSelection } = useRecipesTable(data);

  if (data.length === 0) {
    return <RecipesTable.Empty />;
  }

  return (
    <>
      <RecipesTable.Header
        rowSelection={rowSelection}
        resetRowSelection={() => {
          table.toggleAllRowsSelected(false);
        }}
      />
      <RecipesTable.Content table={table} />
    </>
  );
}

type RecipesTableHeaderProps = {
  rowSelection: RowSelectionState;
  resetRowSelection: () => void;
};

RecipesTable.Header = function Header({
  rowSelection,
  resetRowSelection,
}: RecipesTableHeaderProps) {
  const deleteRecipesMutation = useDeleteRecipesMutation();

  const handleDeleteSelected = () => {
    const selectedIds = Object.keys(rowSelection);
    if (selectedIds.length > 0) {
      deleteRecipesMutation.mutate(
        { ids: selectedIds },
        { onSuccess: resetRowSelection },
      );
    }
  };

  return (
    <PageTitle title="Recipes">
      <div className="flex justify-end gap-2">
        <Button asChild>
          <Link to="/nutrition/recipes/create">Create Recipe</Link>
        </Button>
        <ButtonWithLoading
          isLoading={deleteRecipesMutation.isDelayedPending}
          variant={"destructive"}
          className="w-32"
          disabled={Object.keys(rowSelection).length === 0}
          onClick={handleDeleteSelected}
        >
          Delete selected : {Object.keys(rowSelection).length}
        </ButtonWithLoading>
      </div>
    </PageTitle>
  );
};

type RecipesTableContentProps = {
  table: TableT<RecipeDto>;
};

RecipesTable.Content = function ({ table }: RecipesTableContentProps) {
  return (
    <Card className="flex overflow-auto rounded-md border p-4">
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
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
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow
                key={row.original.id}
                data-state={row.getIsSelected() && "selected"}
                className={cn(row.original.isDeleting ? "opacity-50" : "")}
              >
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id} className="h-12 py-2">
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow className="">
              <TableCell
                colSpan={recipesTableColumns.length}
                className="h-12 text-left"
              />
            </TableRow>
          )}
        </TableBody>
      </Table>
    </Card>
  );
};

RecipesTable.Empty = function Empty() {
  const navigate = useNavigate();
  return (
    <div className="flex flex-col items-center gap-4 text-center">
      <h3 className="text-2xl font-bold tracking-tight">You have no recipes</h3>
      <Button
        className="w-full max-w-md"
        onClick={() => navigate({ to: "/nutrition/recipes/create" })}
      >
        Create a new recipe
      </Button>
    </div>
  );
};

export default RecipesTable;
