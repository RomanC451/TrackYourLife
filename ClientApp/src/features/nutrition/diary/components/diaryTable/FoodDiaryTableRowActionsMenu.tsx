import React from "react";
import { CircularProgress } from "@mui/material";
import { MoreHorizontal } from "lucide-react";
import { useToggle } from "usehooks-ts";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DiaryType, NutritionDiaryDto } from "@/services/openapi";

import useDeleteNutritionDiaryMutation from "../../mutations/useDeleteNutritionDiaryMutation";
import EditFoodDiaryEntryDialog from "../foodDiaryEntryDialogs/EditFoodDiaryEntryDialog";
import EditRecipeDiaryEntryDialog from "../recipeDiaryEntryDialogs/EditRecipeDiaryEntryDialog";

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
