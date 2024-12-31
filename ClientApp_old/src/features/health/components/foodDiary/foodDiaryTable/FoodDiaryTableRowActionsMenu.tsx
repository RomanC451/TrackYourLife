import { MoreHorizontal } from "lucide-react";
import React from "react";
import { useToggle } from "usehooks-ts";
import { Button } from "~/chadcn/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "~/chadcn/ui/dropdown-menu";

import { CircularProgress } from "@mui/material";

import useDeleteNutritionDiaryMutation from "~/features/health/mutations/foodDiaries/useDeleteNutritionDiaryMutation";
import { DiaryType, NutritionDiaryDto } from "~/services/openapi";
import EditFoodDiaryEntryDialog from "../foodDIaryDialogs/EditFoodDiaryEntryDialog";
import EditRecipeDiaryEntryDialog from "../foodDIaryDialogs/EditRecipeDiaryEntryDialog";

type RowActionsMenuProps = {
  diary: NutritionDiaryDto;
};

const FoodDiaryTableRowActionsMenu: React.FC<RowActionsMenuProps> = ({
  diary,
}) => {
  const [editModalState, toggleEditModalState] = useToggle(false);

  const { deleteNutritionDiaryMutation, isPending } =
    useDeleteNutritionDiaryMutation();

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
          <DropdownMenuItem onClick={() => toggleEditModalState()}>
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
      {editModalState ? (
        diary.diaryType == DiaryType.FoodDiary ? (
          <EditFoodDiaryEntryDialog
            diaryId={diary.id}
            dialogState={editModalState}
            toggleDialogState={toggleEditModalState}
          />
        ) : (
          <EditRecipeDiaryEntryDialog
            diaryId={diary.id}
            dialogState={editModalState}
            toggleDialogState={toggleEditModalState}
          />
        )
      ) : null}
    </>
  );
};

export default FoodDiaryTableRowActionsMenu;
