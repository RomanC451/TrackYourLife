import { useCustomMutation } from "@/hooks/useCustomMutation";
import { BooksApi, type CreateBookRequest } from "@/services/openapi";

import { booksQueryKeys } from "../queries/booksQuery";

const booksApi = new BooksApi();

const useCreateBookMutation = () =>
  useCustomMutation({
    mutationFn: (body: CreateBookRequest) =>
      booksApi.createBook(body).then((r) => r.data),
    meta: {
      invalidateQueries: [booksQueryKeys.all],
      onSuccessToast: { type: "success", message: "Book added." },
    },
  });

export default useCreateBookMutation;
