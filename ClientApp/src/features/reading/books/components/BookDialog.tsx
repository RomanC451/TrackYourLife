import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { BookDto } from "@/services/openapi";

import useCreateBookMutation from "../mutations/useCreateBookMutation";
import useUpdateBookMutation from "../mutations/useUpdateBookMutation";
import BookForm, { bookDtoToFormValues } from "./BookForm";
import { bookFormValuesToRequest } from "../schemas/bookFormValuesToRequest";

type BookDialogProps = {
  dialogType: "create" | "edit";
  book?: BookDto;
  onClose: () => void;
};

function BookDialog({ dialogType, book, onClose }: BookDialogProps) {
  const createMutation = useCreateBookMutation();
  const updateMutation = useUpdateBookMutation();

  const isEdit = dialogType === "edit";

  return (
    <Dialog open onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="max-h-[85vh] max-w-lg overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit book" : "Add book"}</DialogTitle>
        </DialogHeader>
        <BookForm
          defaultValues={book ? bookDtoToFormValues(book) : undefined}
          submitLabel={isEdit ? "Save" : "Add book"}
          isSubmitting={
            isEdit ? updateMutation.isPending : createMutation.isPending
          }
          onSubmit={async (values) => {
            const body = bookFormValuesToRequest(values);

            if (isEdit && book) {
              await updateMutation.mutateAsync({ id: book.id, body });
            } else {
              await createMutation.mutateAsync(body);
            }
            onClose();
          }}
        />
      </DialogContent>
    </Dialog>
  );
}

export default BookDialog;
