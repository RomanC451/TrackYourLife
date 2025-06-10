import { CircularProgress } from "@mui/material";
import { flexRender, RowSelectionState } from "@tanstack/react-table";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import useRecipesQuery from "@/features/nutrition/common/queries/useRecipesQuery";

import useDeleteRecipesMutation from "../../mutations/useDeleteRecipesMutation";
import CreateRecipeDialog from "../recipesDialogs/CreateRecipeDialog";
import { recipesTableColumns } from "./recipesTableColumns";
import useRecipesTable from "./useRecipesTable";

function RecipesTable() {
  const { recipesQuery, isPending } = useRecipesQuery();

  const { table, rowSelection } = useRecipesTable(recipesQuery.data ?? []);

  if (isPending.isLoading) {
    return <RecipesTable.Loading />;
  }

  if (recipesQuery.isError) {
    return <div>Error</div>;
  }

  if (recipesQuery.data === undefined) {
    return null;
  }

  if (recipesQuery.data.length === 0) {
    return <RecipesTable.Empty buttonDisabled={!isPending.isLoaded} />;
  }

  return (
    <>
      <RecipesTable.Header
        rowSelection={rowSelection}
        resetRowSelection={() => {
          table.toggleAllRowsSelected(false);
        }}
      />
      <div className="flex overflow-auto rounded-md border">
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
    </>
  );
}

RecipesTable.Header = function TableHeader({
  rowSelection,
  resetRowSelection,
}: {
  rowSelection: RowSelectionState;
  resetRowSelection: () => void;
}) {
  const { deleteRecipesMutation, isPending } = useDeleteRecipesMutation();

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
      <div className="flex flex-wrap justify-end gap-2">
        <CreateRecipeDialog>
          <Button className="w-32">Create recipe</Button>
        </CreateRecipeDialog>

        <ButtonWithLoading
          isLoading={isPending.isLoading}
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

RecipesTable.Loading = function () {
  return (
    <div className="flex flex-grow items-center justify-center">
      <CircularProgress color="inherit" />
    </div>
  );
};

RecipesTable.Empty = function ({
  buttonDisabled = false,
}: {
  buttonDisabled?: boolean;
}) {
  return (
    <div className="flex flex-col items-center gap-1 text-center">
      <h3 className="text-2xl font-bold tracking-tight">You have no recipes</h3>
      <CreateRecipeDialog>
        <Button className="mt-4" disabled={buttonDisabled}>
          Create a new recipe
        </Button>
      </CreateRecipeDialog>
    </div>
  );
};

export default RecipesTable;
