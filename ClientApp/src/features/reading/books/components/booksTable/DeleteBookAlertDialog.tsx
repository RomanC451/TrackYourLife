"use no memo";

import { useState } from "react";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Input } from "@/components/ui/input";
import useDeleteBookMutation from "@/features/reading/books/mutations/useDeleteBookMutation";
import { BookDto } from "@/services/openapi";

type DeleteBookAlertDialogProps = {
  book: BookDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

function DeleteBookAlertDialog({
  book,
  open,
  onOpenChange,
}: DeleteBookAlertDialogProps) {
  const deleteBookMutation = useDeleteBookMutation();
  const [confirmTitle, setConfirmTitle] = useState("");
  const isConfirmed = confirmTitle === book.title;

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      setConfirmTitle("");
    }
    onOpenChange(nextOpen);
  };

  return (
    <AlertDialog open={open} onOpenChange={handleOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete book</AlertDialogTitle>
          <AlertDialogDescription>
            This will permanently delete the book and all of its reading notes.
            This action cannot be undone. Type &ldquo;{book.title}&rdquo; to
            confirm deletion.
          </AlertDialogDescription>
        </AlertDialogHeader>

        <Input
          id="confirm-book-title"
          value={confirmTitle}
          onChange={(e) => setConfirmTitle(e.target.value)}
          placeholder="Book title"
          autoComplete="off"
          autoFocus
        />

        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction asChild>
            <ButtonWithLoading
              variant="destructive"
              className="min-w-[100px]"
              isLoading={deleteBookMutation.isDelayedPending}
              disabled={deleteBookMutation.isPending || !isConfirmed}
              onClick={(e) => {
                e.preventDefault();
                deleteBookMutation.mutate(book.id, {
                  onSuccess: () => handleOpenChange(false),
                });
              }}
            >
              Delete
            </ButtonWithLoading>
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

export default DeleteBookAlertDialog;
