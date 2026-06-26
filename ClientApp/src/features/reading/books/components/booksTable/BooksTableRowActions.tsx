"use no memo";

import { useState } from "react";
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
import useDeleteBookMutation from "@/features/reading/books/mutations/useDeleteBookMutation";
import { BookDto } from "@/services/openapi";

import DeleteBookAlertDialog from "./DeleteBookAlertDialog";

type BooksTableRowActionsProps = {
  book: BookDto;
};

function BooksTableRowActions({ book }: BooksTableRowActionsProps) {
  const deleteBookMutation = useDeleteBookMutation();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  const handleOpenChange = (open: boolean) => {
    if (open) {
      router.preloadRoute({
        to: "/reading/books/edit/$bookId",
        params: { bookId: book.id },
      });
    }
  };

  return (
    <>
      <DropdownMenu modal={false} onOpenChange={handleOpenChange}>
        <DropdownMenuTrigger
          asChild
          disabled={deleteBookMutation.isPending}
        >
          <Button
            disabled={deleteBookMutation.isPending}
            variant="ghost"
            className="p-0"
          >
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="size-2" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem asChild>
            <Link to="/reading/books/edit/$bookId" params={{ bookId: book.id }}>
              Edit
            </Link>
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            className="text-destructive focus:text-destructive"
            disabled={deleteBookMutation.isPending}
            onSelect={() => setDeleteDialogOpen(true)}
          >
            Delete
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>

      <DeleteBookAlertDialog
        book={book}
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
      />
    </>
  );
}

export default BooksTableRowActions;
