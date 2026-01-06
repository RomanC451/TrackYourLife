import React from "react";
import { Link } from "@tanstack/react-router";
import { MoreHorizontal } from "lucide-react";

import { router } from "@/App";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { RecipeDto } from "@/services/openapi";

import useDeleteRecipeMutation from "../../mutations/useDeleteRecipeMutation";

type RowActionsMenuProps = {
  recipe: RecipeDto;
};

const RecipesTableRowActionsMenu: React.FC<RowActionsMenuProps> = ({
  recipe,
}) => {
  const deleteRecipeMutation = useDeleteRecipeMutation({
    recipeId: recipe.id,
  });

  const handleOpenChange = (open: boolean) => {
    if (open) {
      router.preloadRoute({
        to: "/nutrition/recipes/edit/$recipeId",
        params: { recipeId: recipe.id },
      });
    }
  };

  return (
    <DropdownMenu modal={false} onOpenChange={handleOpenChange}>
      <DropdownMenuTrigger
        asChild
        disabled={deleteRecipeMutation.isPending}
        className=""
      >
        <Button
          disabled={deleteRecipeMutation.isPending}
          variant="ghost"
          className="p-0"
        >
          <span className="sr-only">Open menu</span>
          <MoreHorizontal className="size-2" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem asChild>
          <Link
            to="/nutrition/recipes/edit/$recipeId"
            params={{ recipeId: recipe.id }}
          >
            Edit
          </Link>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => {
            deleteRecipeMutation.mutate({ recipe });
          }}
        >
          Remove
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default RecipesTableRowActionsMenu;
