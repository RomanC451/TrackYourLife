import { MoreHorizontal } from "lucide-react";
import React from "react";
import { Button } from "~/chadcn/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "~/chadcn/ui/dropdown-menu";

import { CircularProgress } from "@mui/material";

import { useRecipesTableContext } from "~/features/health/contexts/RecipesTableContext";
import useDeleteRecipeMutation from "~/features/health/mutations/recipes/useDeleteRecipeMutation";
import { RecipeDto } from "~/services/openapi";

type RowActionsMenuProps = {
  recipe: RecipeDto;
};

const RecipesTableRowActionsMenu: React.FC<RowActionsMenuProps> = ({
  recipe,
}) => {
  const { openEditRecipeModal } = useRecipesTableContext();

  const { deleteRecipeMutation, isPending } = useDeleteRecipeMutation();

  return (
    <>
      <DropdownMenu modal={false}>
        <DropdownMenuTrigger asChild disabled={!isPending.isLoaded}>
          {isPending.isLoading ? (
            <div className="grid h-8 w-8 place-items-center">
              <CircularProgress size={25} />
            </div>
          ) : (
            <Button variant="ghost" className="h-8 w-8 p-0">
              <span className="sr-only">Open menu</span>
              <MoreHorizontal className="h-4 w-4" />
            </Button>
          )}
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => openEditRecipeModal(recipe.id)}>
            Edit
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onClick={() => {
              deleteRecipeMutation.mutate(recipe);
            }}
          >
            Remove
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </>
  );
};

export default RecipesTableRowActionsMenu;
