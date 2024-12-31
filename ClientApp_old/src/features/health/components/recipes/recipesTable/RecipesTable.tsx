import { CircularProgress } from "@mui/material";
import { flexRender } from "@tanstack/react-table";
import { Button } from "~/chadcn/ui/button";
import { Card, CardTitle } from "~/chadcn/ui/card";
import { ScrollArea, ScrollBar } from "~/chadcn/ui/scroll-area";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "~/chadcn/ui/table";
import useRecipesTable from "~/features/health/hooks/recipes/useRecipesTable";
import useRecipesQuery from "~/features/health/queries/recipes/useRecipesQuery";
import CreateRecipeDialog from "../recipeDialogs/CreateRecipeDialog";
import { recipesTableColumns } from "./recipesTableColumns";

type RecipesListProps = {};

function RecipesTable({}: RecipesListProps) {
  const { recipesQuery, isPending } = useRecipesQuery();

  const { table } = useRecipesTable(recipesQuery.data ?? []);

  if (isPending.isLoading) {
    return <RecipesTable.Loading />;
  }

  if (recipesQuery.data === undefined) {
    return null;
  }

  if (recipesQuery.data.length === 0) {
    return <RecipesTable.Empty buttonDisabled={!isPending.isLoaded} />;
  }

  return (
    <>
      <RecipesTable.Header />
      <ScrollArea className="rounded-lg border-[1px]">
        <div className="flex flex-grow rounded-md border">
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
                    key={row.id}
                    data-state={row.getIsSelected() && "selected"}
                    className="h-12"
                  >
                    {row.getVisibleCells().map((cell) => (
                      <TableCell key={cell.id} className="h-12 py-2">
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext(),
                        )}
                      </TableCell>
                    ))}
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell
                    colSpan={recipesTableColumns.length}
                    className="h-12 text-left"
                  />
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
        <ScrollBar orientation="horizontal" />
      </ScrollArea>
    </>
  );
}

RecipesTable.Header = function () {
  return (
    <div className="flex items-center justify-between">
      <CardTitle>Recipes</CardTitle>
      <CreateRecipeDialog>
        <Button>Create recipe</Button>
      </CreateRecipeDialog>
    </div>
  );
};

RecipesTable.Loading = function () {
  return (
    <Card className="flex flex-grow">
      <div className="flex flex-grow items-center justify-center">
        <CircularProgress />
      </div>
    </Card>
  );
};

RecipesTable.Empty = function ({
  buttonDisabled = false,
}: {
  buttonDisabled?: boolean;
}) {
  return (
    <Card className="flex flex-1 items-center justify-center rounded-lg border border-dashed shadow-sm">
      <div className="flex flex-col items-center gap-1 text-center">
        <h3 className="text-2xl font-bold tracking-tight">
          You have no recipes
        </h3>
        <CreateRecipeDialog>
          <Button className="mt-4" disabled={buttonDisabled}>
            Create a new recipe
          </Button>
        </CreateRecipeDialog>
      </div>
    </Card>
  );
};

export default RecipesTable;
