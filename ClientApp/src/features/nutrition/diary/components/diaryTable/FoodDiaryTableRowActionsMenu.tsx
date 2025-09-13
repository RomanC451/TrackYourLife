import React from "react";
import { useNavigate } from "@tanstack/react-router";
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
import Spinner from "@/components/ui/spinner";
import { NutritionDiaryDto } from "@/services/openapi";

import useDeleteNutritionDiaryMutation from "../../mutations/useDeleteNutritionDiaryMutation";

type RowActionsMenuProps = {
  diary: NutritionDiaryDto;
};

const FoodDiaryTableRowActionsMenu: React.FC<RowActionsMenuProps> = ({
  diary,
}) => {
  const deleteNutritionDiaryMutation = useDeleteNutritionDiaryMutation();

  const navigate = useNavigate();

  return (
    <DropdownMenu modal={false}>
      <DropdownMenuTrigger
        asChild
        disabled={deleteNutritionDiaryMutation.isPending}
      >
        {deleteNutritionDiaryMutation.isDelayedPending ? (
          <div className="grid h-8 w-8 place-items-center">
            <Spinner className="size-5" />
          </div>
        ) : (
          <Button
            disabled={diary.isDeleting || diary.isLoading}
            variant="ghost"
            className="h-8 w-8 p-0"
          >
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        )}
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem
          onClick={() => {
            navigate({
              to: "/nutrition/diary/foodDiary/edit/$diaryId",
              params: {
                diaryId: diary.id,
              },
            });
          }}
          onMouseEnter={() => {
            router.preloadRoute({
              to: "/nutrition/diary/foodDiary/edit/$diaryId",
              params: {
                diaryId: diary.id,
              },
            });
          }}
          onTouchStart={() => {
            router.preloadRoute({
              to: "/nutrition/diary/foodDiary/edit/$diaryId",
              params: {
                diaryId: diary.id,
              },
            });
          }}
        >
          Edit
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => {
            deleteNutritionDiaryMutation.mutate(diary);
          }}
        >
          Remove
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default FoodDiaryTableRowActionsMenu;
