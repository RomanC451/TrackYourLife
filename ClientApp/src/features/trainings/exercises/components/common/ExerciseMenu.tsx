import { ButtonHTMLAttributes, useEffect, useRef, useState } from "react";
import { Separator } from "@radix-ui/react-dropdown-menu";
import { useNavigate } from "@tanstack/react-router";
import { Edit, Eye, MoreVertical, Trash2 } from "lucide-react";

import { router } from "@/App";
import { Button } from "@/components/ui/button";
import Spinner from "@/components/ui/spinner";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";
import ForceDeleteExerciseAlertDialog from "./ForceDeleteExerciseAlertDialog";

export function ExerciseMenu({
  exercise,
  disabled,
  defaultOpen,
  onClose,
  onOpen,
}: {
  exercise: ExerciseDto;
  disabled?: boolean;
  defaultOpen?: boolean;
  onClose?: () => void;
  onOpen?: () => void;
}) {
  const navigate = useNavigate();

  const deleteExerciseMutation = useDeleteExerciseMutation();

  const [showForceDeleteAlert, setShowForceDeleteAlert] = useState(false);

  const [isOpen, setIsOpen] = useState(false);
  const [openAbove, setOpenAbove] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);

  // Use controlled or internal state
  const actualIsOpen = defaultOpen ?? isOpen;

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (
        menuRef.current &&
        !menuRef.current.contains(event.target as Node) &&
        buttonRef.current &&
        !buttonRef.current.contains(event.target as Node)
      ) {
        if (onClose) onClose();
        else setIsOpen(false);
      }
    }

    if (actualIsOpen) {
      document.addEventListener("mousedown", handleClickOutside);
      return () =>
        document.removeEventListener("mousedown", handleClickOutside);
    }
  }, [actualIsOpen, onClose]);

  const onDelete = () => {
    deleteExerciseMutation.mutate({
      id: exercise.id,
      forceDelete: false,
      name: exercise.name,
      onShowAlert: () => {
        setShowForceDeleteAlert(true);
      },
    });
  };

  const handleMenuAction = (action: () => void) => {
    action();
    if (onClose) onClose();
    else setIsOpen(false);
  };

  const handleMenuOpen = (e: React.MouseEvent | React.KeyboardEvent) => {
    e.stopPropagation();
    if (buttonRef.current) {
      const buttonRect = buttonRef.current.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      setOpenAbove(
        buttonRect.top + buttonRect.height / 2 > viewportHeight / 1.75,
      );
    }
    if (actualIsOpen) {
      if (onClose) onClose();
      else setIsOpen(false);
    } else if (onOpen) onOpen();
    else setIsOpen(true);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      handleMenuOpen(e);
    }
  };

  const handleMenuKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Escape") {
      e.preventDefault();
      if (onClose) onClose();
      else setIsOpen(false);
    }
  };

  const getSpinnerColor = () => {
    if (exercise.isDeleting) return "fill-red-700";
    if (exercise.isLoading) return "fill-green-700";
    return "fill-primary";
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
        onKeyDown={handleKeyDown}
        aria-haspopup="true"
        aria-expanded={actualIsOpen}
        aria-label="Exercise menu"
      >
        {exercise.isDeleting || exercise.isLoading ? (
          <Spinner className={cn("size-5")} color={getSpinnerColor()} />
        ) : (
          <MoreVertical className="size-3" />
        )}
      </Button>

      {actualIsOpen && (
        <div
          ref={menuRef}
          role="menu"
          aria-orientation="vertical"
          className={cn(
            "absolute right-0 z-100 w-48 rounded-md border bg-popover py-1 shadow-lg",
            openAbove ? "bottom-full mb-1" : "top-full mt-1",
          )}
          onClick={(e) => e.stopPropagation()}
          onKeyDown={handleMenuKeyDown}
          tabIndex={-1}
        >
          <ExerciseMenuActionButton
            icon={<Eye className="mr-2 h-4 w-4" />}
            text="View Exercise"
            className="hover:bg-accent hover:text-accent-foreground"
            onClick={() => {
              navigate({
                to: "/trainings/workouts/exercises/info/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
            onMouseEnter={() => {
              router.preloadRoute({
                to: "/trainings/workouts/exercises/info/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
            onTouchStart={() => {
              router.preloadRoute({
                to: "/trainings/workouts/exercises/info/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
          />

          <Separator className="my-1 h-px w-full bg-accent" />

          <ExerciseMenuActionButton
            icon={<Edit className="mr-2 h-4 w-4" />}
            text="Edit Exercise"
            className={cn(
              "hover:enabled:bg-accent hover:enabled:text-accent-foreground disabled:opacity-50",
            )}
            onClick={() => {
              navigate({
                to: "/trainings/workouts/exercises/edit/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
            onMouseEnter={() => {
              router.preloadRoute({
                to: "/trainings/workouts/exercises/edit/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
            onTouchStart={() => {
              router.preloadRoute({
                to: "/trainings/workouts/exercises/edit/$exerciseId",
                params: { exerciseId: exercise.id },
              });
            }}
          />

          <Separator className="my-1 h-px w-full bg-accent" />

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

      {showForceDeleteAlert && (
        <ForceDeleteExerciseAlertDialog
          id={exercise.id}
          name={exercise.name}
          onSuccess={() => {
            setShowForceDeleteAlert(false);
          }}
          onCancel={() => {
            setShowForceDeleteAlert(false);
          }}
        />
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
