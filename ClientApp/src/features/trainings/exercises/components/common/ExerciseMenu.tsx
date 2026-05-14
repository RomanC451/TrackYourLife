import {
  type ButtonHTMLAttributes,
  type KeyboardEvent,
  type MouseEvent as ReactMouseEvent,
  type ReactNode,
  useCallback,
  useEffect,
  useRef,
  useState,
} from "react";
import { Separator } from "@radix-ui/react-dropdown-menu";
import { Link } from "@tanstack/react-router";
import { BarChart3, Edit, Eye, MoreVertical, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import Spinner from "@/components/ui/spinner";
import { cn } from "@/lib/utils";
import { ExerciseDto } from "@/services/openapi";

import useDeleteExerciseMutation from "../../mutations/useDeleteExerciseMutation";
import ForceDeleteExerciseAlertDialog from "./ForceDeleteExerciseAlertDialog";

type ExerciseMenuProps = {
  exercise: ExerciseDto;
  disabled?: boolean;
  /** When set, open state is driven by this value; otherwise internal state is used. */
  defaultOpen?: boolean;
  onClose?: () => void;
  onOpen?: () => void;
};

/** Literal `to` + params/search for menu links (avoids TanStack widening `to` to `string`). */
export type ExerciseMenuLinkRoute =
  | {
      to: "/trainings/workouts/exercises/info/$exerciseId";
      params: { exerciseId: string };
    }
  | {
      to: "/trainings/exercises/$exerciseId/stats";
      params: { exerciseId: string };
      search: { range: "TwelveWeeks" };
    }
  | {
      to: "/trainings/exercises/edit/$exerciseId";
      params: { exerciseId: string };
    };

function spinnerColorFor(exercise: ExerciseDto) {
  if (exercise.isDeleting) return "fill-red-700";
  if (exercise.isLoading) return "fill-green-700";
  return "fill-primary";
}

function ExerciseMenuActionButton({
  icon,
  text,
  onClick,
  className,
  ...props
}: ButtonHTMLAttributes<HTMLButtonElement> & {
  icon: ReactNode;
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

export function ExerciseMenuActionButtonLink({
  route,
  icon,
  text,
  className,
}: {
  route: ExerciseMenuLinkRoute;
  icon: ReactNode;
  text: string;
  className?: string;
}) {
  return (
    <Link
      to={route.to}
      params={route.params}
      {...("search" in route ? { search: route.search } : {})}
      preload="intent"
      role="menuitem"
      className={cn(
        "m-auto flex w-[calc(100%-6px)] items-center rounded-md p-2 px-3 py-2 text-left text-sm text-foreground no-underline outline-none",
        className,
      )}
    >
      {icon} {text}
    </Link>
  );
}

export function ExerciseMenu({
  exercise,
  disabled,
  defaultOpen,
  onClose,
  onOpen,
}: ExerciseMenuProps) {
  const deleteExerciseMutation = useDeleteExerciseMutation();
  const [showForceDeleteAlert, setShowForceDeleteAlert] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [openAbove, setOpenAbove] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);

  const actualIsOpen = defaultOpen ?? isOpen;

  const dismiss = useCallback(() => {
    if (onClose) {
      onClose();
    } else {
      setIsOpen(false);
    }
  }, [onClose]);

  useEffect(() => {
    function handleClickOutside(event: globalThis.MouseEvent) {
      if (
        menuRef.current &&
        !menuRef.current.contains(event.target as Node) &&
        buttonRef.current &&
        !buttonRef.current.contains(event.target as Node)
      ) {
        dismiss();
      }
    }

    if (actualIsOpen) {
      document.addEventListener("mousedown", handleClickOutside);
      return () =>
        document.removeEventListener("mousedown", handleClickOutside);
    }
  }, [actualIsOpen, dismiss]);

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
    dismiss();
  };

  const handleMenuOpen = (e: ReactMouseEvent | KeyboardEvent) => {
    e.stopPropagation();
    if (buttonRef.current) {
      const buttonRect = buttonRef.current.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      setOpenAbove(
        buttonRect.top + buttonRect.height / 2 > viewportHeight / 1.75,
      );
    }
    if (actualIsOpen) {
      dismiss();
    } else if (onOpen) {
      onOpen();
    } else {
      setIsOpen(true);
    }
  };

  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      handleMenuOpen(e);
    }
  };

  const handleMenuKeyDown = (e: KeyboardEvent) => {
    if (e.key === "Escape") {
      e.preventDefault();
      dismiss();
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
        onKeyDown={handleKeyDown}
        aria-haspopup="true"
        aria-expanded={actualIsOpen}
        aria-label="Exercise menu"
      >
        {exercise.isDeleting || exercise.isLoading ? (
          <Spinner className="size-5" color={spinnerColorFor(exercise)} />
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
          <ExerciseMenuActionButtonLink
            route={{
              to: "/trainings/workouts/exercises/info/$exerciseId",
              params: { exerciseId: exercise.id },
            }}
            icon={<Eye className="mr-2 h-4 w-4" />}
            text="View Exercise"
            className="hover:bg-accent hover:text-accent-foreground"
          />

          <Separator className="my-1 h-px w-full bg-accent" />

          <ExerciseMenuActionButtonLink
            route={{
              to: "/trainings/exercises/$exerciseId/stats",
              params: { exerciseId: exercise.id },
              search: { range: "TwelveWeeks" },
            }}
            icon={<BarChart3 className="mr-2 h-4 w-4" />}
            text="View Stats"
            className="hover:bg-accent hover:text-accent-foreground"
          />

          <Separator className="my-1 h-px w-full bg-accent" />

          <ExerciseMenuActionButtonLink
            route={{
              to: "/trainings/exercises/edit/$exerciseId",
              params: { exerciseId: exercise.id },
            }}
            icon={<Edit className="mr-2 h-4 w-4" />}
            text="Edit Exercise"
            className="hover:enabled:bg-accent hover:enabled:text-accent-foreground disabled:opacity-50"
          />

          <Separator className="my-1 h-px w-full bg-accent" />

          <ExerciseMenuActionButton
            icon={<Trash2 className="mr-2 h-4 w-4" />}
            text="Delete Exercise"
            onClick={() => handleMenuAction(onDelete)}
            className="m-auto flex w-[calc(100%-6px)] items-center rounded-md p-2 px-3 py-2 text-left text-sm font-bold text-destructive hover:enabled:bg-destructive hover:enabled:text-destructive-foreground disabled:opacity-50"
          />
        </div>
      )}

      {showForceDeleteAlert && (
        <ForceDeleteExerciseAlertDialog
          id={exercise.id}
          name={exercise.name}
          onSuccess={() => setShowForceDeleteAlert(false)}
          onCancel={() => setShowForceDeleteAlert(false)}
        />
      )}
    </div>
  );
}

export default ExerciseMenu;
