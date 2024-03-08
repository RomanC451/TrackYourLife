import { MoreHorizontal } from "lucide-react";
import React from "react";
import { toast } from "sonner";
import { useToggle } from "usehooks-ts";
import { Button } from "~/chadcn/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "~/chadcn/ui/dropdown-menu";
import { toastDefaultServerError } from "~/data/apiSettings";
import { useApiRequests } from "~/hooks/useApiRequests";
import useDelayedLoading from "~/hooks/useDelayedLoading";

import { CircularProgress } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import {
  deleteFoodDiaryRequest,
  postFoodDiaryRequest,
  TGetFoodDiaryListResponse,
} from "../../../../requests/";
import EditFoodDiaryEntryDialog from "../../foodDiaryForms/EditFoodDiaryEntryDialog";
import { FoodDiaryEntry } from "./columns";

type RowActionsMenuProps = {
  diaryEntry: FoodDiaryEntry;
};

const RowActionsMenu: React.FC<RowActionsMenuProps> = ({ diaryEntry }) => {
  const [editModalState, toggleEditModalState] = useToggle(false);

  const { fetchRequest } = useApiRequests();

  const queryClient = useQueryClient();

  const addFoodDiaryMutation = useMutation({
    mutationFn: () =>
      postFoodDiaryRequest(fetchRequest, {
        foodId: diaryEntry.foodId,
        mealType: diaryEntry.meal,
        servingSizeId: diaryEntry.servingSize.id,
        quantity: diaryEntry.nrOfServings,
        date: diaryEntry.date,
      }),
    onSuccess: () => {
      toast(`${diaryEntry.name} (${diaryEntry.brandName})`, {
        description: `${diaryEntry.quantity}  has been added on ${diaryEntry.meal}`,
      });

      queryClient.invalidateQueries({
        queryKey: ["foodDiary", diaryEntry.date],
      });

      queryClient.setQueryData(
        ["foodDiary", diaryEntry.date],
        (oldData: TGetFoodDiaryListResponse) => {
          const newData = {
            ...oldData,
            [diaryEntry.meal.toLowerCase()]: [
              ...oldData[
                diaryEntry.meal.toLowerCase() as keyof TGetFoodDiaryListResponse
              ],
              {
                id: diaryEntry.id,
                food: diaryEntry.food,
                mealType: diaryEntry.meal,
                quantity: diaryEntry.quantity,
                servingSize: diaryEntry.servingSize,
                date: diaryEntry.date,
              },
            ],
          };

          return newData;
        },
      );
    },
    onError: toastDefaultServerError,
  });

  const deleteFoodDiaryMutation = useMutation({
    mutationFn: () => deleteFoodDiaryRequest(fetchRequest, diaryEntry.id),
    onSuccess: () => {
      toast(`${diaryEntry.name} (${diaryEntry.brandName})`, {
        description: `Has been removed from ${diaryEntry.meal}`,
        action: {
          label: "Undo",
          onClick: () => {
            addFoodDiaryMutation.mutate();
          },
        },
      });
      queryClient.invalidateQueries({
        queryKey: ["foodDiary", diaryEntry.date],
      });

      queryClient.setQueryData(
        ["foodDiary", diaryEntry.date],
        (oldData: TGetFoodDiaryListResponse) => {
          const newData = {
            ...oldData,
            [diaryEntry.meal.toLowerCase()]: oldData[
              diaryEntry.meal.toLowerCase() as keyof TGetFoodDiaryListResponse
            ].filter((item) => item.id != diaryEntry.id),
          };

          return newData;
        },
      );
    },
    onError: toastDefaultServerError,
  });

  const loadingState = useDelayedLoading(
    100,
    deleteFoodDiaryMutation.isPending,
  );

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger
          asChild
          disabled={deleteFoodDiaryMutation.isPending}
        >
          {loadingState.isLoading ? (
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
              deleteFoodDiaryMutation.mutate();
            }}
          >
            Remove
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
      {editModalState ? (
        <EditFoodDiaryEntryDialog
          diaryEntry={diaryEntry}
          dialogState={editModalState}
          toggleDialogState={toggleEditModalState}
        />
      ) : null}
    </>
  );
};

export default RowActionsMenu;
