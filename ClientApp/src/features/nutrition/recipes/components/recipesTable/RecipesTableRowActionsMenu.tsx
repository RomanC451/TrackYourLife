import React from "react";
import { MoreHorizontal } from "lucide-react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { RecipeDto } from "@/services/openapi";

import { useRecipesTableContext } from "../../contexts/RecipesTableContext";
import useDeleteRecipeMutation from "../../mutations/useDeleteRecipeMutation";

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
        <DropdownMenuTrigger
          asChild
          disabled={!isPending.isLoaded}
          className=""
        >
          <ButtonWithLoading
            isLoading={isPending.isStarting}
            variant="ghost"
            className="p-0"
          >
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="size-2" />
          </ButtonWithLoading>
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
