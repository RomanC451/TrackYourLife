import { ButtonHTMLAttributes, useEffect, useRef, useState } from "react";
import { Separator } from "@radix-ui/react-dropdown-menu";
import { Edit, Eye, MoreVertical, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import Spinner from "@/components/ui/spinner";
import { useTrainingsDialogsContext } from "@/features/trainings/common/contexts/TrainingsDialogsContextProvider";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";

export function ExerciseMenu({
  exercise,
  onSuccessDelete,
  onSuccessEdit,
  isOpen: controlledIsOpen,
  onRequestClose,
  onOpenMenu,
  disabled,
}: {
  exercise: ExerciseDto;
  onSuccessDelete: () => void;
  onSuccessEdit: (exercise: Partial<ExerciseDto>) => void;
  isOpen?: boolean;
  onRequestClose?: () => void;
  onOpenMenu?: () => void;
  disabled?: boolean;
}) {
  const { deleteExerciseMutation } = useDeleteExerciseMutation();

  const { setExerciseToForceDelete, setExerciseToEdit, setExerciseToView } =
    useTrainingsDialogsContext();

  const [isOpen, setIsOpen] = useState(false);
  const [openAbove, setOpenAbove] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);

  // Use controlled or internal state
  const actualIsOpen =
    controlledIsOpen !== undefined ? controlledIsOpen : isOpen;

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (
        menuRef.current &&
        !menuRef.current.contains(event.target as Node) &&
        buttonRef.current &&
        !buttonRef.current.contains(event.target as Node)
      ) {
        if (onRequestClose) onRequestClose();
        else setIsOpen(false);
      }
    }

    if (actualIsOpen) {
      document.addEventListener("mousedown", handleClickOutside);
      return () =>
        document.removeEventListener("mousedown", handleClickOutside);
    }
  }, [actualIsOpen, onRequestClose]);

  const onView = () => {
    setExerciseToView(exercise);
  };

  const onEdit = () => {
    setExerciseToEdit({
      exercise: exercise,
      onSuccessEdit: onSuccessEdit,
      onCancelEdit: () => {},
    });
  };

  const onDelete = () => {
    deleteExerciseMutation.mutate(
      {
        id: exercise.id,
        forceDelete: false,
        name: exercise.name,
        onShowAlert: () => {
          setExerciseToForceDelete({
            exercise: exercise,
            onSuccessDelete: onSuccessDelete,
            onCancelDelete: () => {},
          });
        },
      },
      {
        onSuccess: onSuccessDelete,
      },
    );
  };

  const handleMenuAction = (action: () => void) => {
    action();
    if (onRequestClose) onRequestClose();
    else setIsOpen(false);
  };

  const handleMenuOpen = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (buttonRef.current) {
      const buttonRect = buttonRef.current.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      setOpenAbove(buttonRect.top + buttonRect.height / 2 > viewportHeight / 2);
    }
    if (actualIsOpen) {
      if (onRequestClose) onRequestClose();
      else setIsOpen(false);
    } else {
      if (onOpenMenu) onOpenMenu();
      else setIsOpen(true);
    }
  };

  return (
    <div className="relative">
      <Button
        ref={buttonRef}
        variant="ghost"
        size="icon"
        type="button"
        disabled={disabled || exercise.isDeleting || exercise.isLoading}
        onClick={handleMenuOpen}
      >
        {exercise.isDeleting || exercise.isLoading ? (
          <Spinner
            className={cn("size-5")}
            color={
              exercise.isDeleting
                ? "fill-red-700"
                : exercise.isLoading
                  ? "fill-green-700"
                  : "fill-primary"
            }
          />
        ) : (
          <MoreVertical className="size-3" />
        )}
      </Button>

      {actualIsOpen && (
        <div
          ref={menuRef}
          className={cn(
            "absolute right-0 z-[100] w-48 rounded-md border bg-popover py-1 shadow-lg",
            openAbove ? "bottom-full mb-1" : "top-full mt-1",
          )}
          onClick={(e) => e.stopPropagation()}
        >
          <ExerciseMenuActionButton
            icon={<Eye className="mr-2 h-4 w-4" />}
            text="View Exercise"
            onClick={() => handleMenuAction(onView)}
            className="hover:bg-accent hover:text-accent-foreground"
          />
          <Separator className="my-1 h-[1px] w-full bg-accent" />
          <ExerciseMenuActionButton
            icon={<Edit className="mr-2 h-4 w-4" />}
            text="Edit Exercise"
            onClick={() => handleMenuAction(onEdit)}
            className={cn(
              "hover:enabled:bg-accent hover:enabled:text-accent-foreground disabled:opacity-50",
            )}
          />
          <Separator className="my-1 h-[1px] w-full bg-accent" />

          <ExerciseMenuActionButton
            icon={<Trash2 className="mr-2 h-4 w-4" />}
            text="Delete Exercise"
            onClick={() => handleMenuAction(onDelete)}
            className={cn(
              "m-auto flex w-[calc(100%-6px)] items-center rounded-md p-2 px-3 py-2 text-left text-sm font-bold text-destructive hover:enabled:bg-destructive hover:enabled:text-destructive-foreground disabled:opacity-50",
            )}
          />
        </div>
      )}
    </div>
  );
}

function ExerciseMenuActionButton({
  icon,
  text,
  onClick,
  className,
  ...props
}: ButtonHTMLAttributes<HTMLButtonElement> & {
  icon: React.ReactNode;
  text: string;
}) {
  return (
    <button
      className={cn(
        "m-auto flex w-[calc(100%-6px)] items-center rounded-md p-2 px-3 py-2 text-left text-sm",
        className,
      )}
      type="button"
      onClick={onClick}
      {...props}
    >
      {icon} {text}
    </button>
  );
}

export default ExerciseMenu;
