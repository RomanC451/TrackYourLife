import { useCustomMutation } from "@/hooks/useCustomMutation";
import { BooksApi, type UpdateBookRequest } from "@/services/openapi";

import { booksQueryKeys } from "../queries/booksQuery";

const booksApi = new BooksApi();

const useUpdateBookMutation = () =>
  useCustomMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateBookRequest }) =>
      booksApi.updateBook(id, body),
    meta: {
      invalidateQueries: [booksQueryKeys.all],
      onSuccessToast: { type: "success", message: "Book updated." },
    },
  });

export default useUpdateBookMutation;
