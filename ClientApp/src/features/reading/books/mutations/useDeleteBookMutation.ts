import { useCustomMutation } from "@/hooks/useCustomMutation";
import { BooksApi } from "@/services/openapi";

import { booksQueryKeys } from "../queries/booksQuery";

const booksApi = new BooksApi();

const useDeleteBookMutation = () =>
  useCustomMutation({
    mutationFn: (id: string) => booksApi.deleteBook(id),
    meta: {
      invalidateQueries: [booksQueryKeys.all],
      onSuccessToast: { type: "success", message: "Book deleted." },
    },
  });

export default useDeleteBookMutation;
