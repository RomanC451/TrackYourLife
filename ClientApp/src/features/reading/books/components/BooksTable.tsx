"use no memo";

import { useState } from "react";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import BookStatusBadge from "@/features/reading/components/BookStatusBadge";
import { BookSortField, BookStatus } from "@/services/openapi";

import useDeleteBookMutation from "../mutations/useDeleteBookMutation";
import { booksQueryOptions } from "../queries/booksQuery";

function BooksTable() {
  const navigate = useNavigate();
  const [statusFilter, setStatusFilter] = useState<BookStatus | "all">("all");
  const [sortBy, setSortBy] = useState<BookSortField>(BookSortField.Title);
  const [sortDescending, setSortDescending] = useState(false);

  const { data } = useSuspenseQuery(
    booksQueryOptions.list({
      status: statusFilter === "all" ? undefined : statusFilter,
      sortBy,
      sortDescending,
    }),
  );

  const deleteBookMutation = useDeleteBookMutation();

  return (
    <>
      <PageTitle title="Books">
        <Button asChild>
          <Link to="/books/create">Add book</Link>
        </Button>
      </PageTitle>

      <div className="mb-4 flex flex-wrap gap-3">
        <Select
          value={statusFilter}
          onValueChange={(v) => setStatusFilter(v as BookStatus | "all")}
        >
          <SelectTrigger className="w-40">
            <SelectValue placeholder="Filter status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            {Object.values(BookStatus).map((s) => (
              <SelectItem key={s} value={s}>
                {s}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Select
          value={sortBy}
          onValueChange={(v) => setSortBy(v as BookSortField)}
        >
          <SelectTrigger className="w-40">
            <SelectValue placeholder="Sort by" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={BookSortField.Title}>Title</SelectItem>
            <SelectItem value={BookSortField.Author}>Author</SelectItem>
            <SelectItem value={BookSortField.StartingDate}>Start date</SelectItem>
            <SelectItem value={BookSortField.FinishDate}>Finish date</SelectItem>
            <SelectItem value={BookSortField.Rating}>Rating</SelectItem>
          </SelectContent>
        </Select>
        <Button
          variant="outline"
          onClick={() => setSortDescending((d) => !d)}
        >
          {sortDescending ? "Descending" : "Ascending"}
        </Button>
      </div>

      {data.length === 0 ? (
        <p className="text-muted-foreground">No books yet. Add your first book.</p>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Title</TableHead>
              <TableHead>Author</TableHead>
              <TableHead>Progress</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.map((book) => (
              <TableRow key={book.id}>
                <TableCell>
                  <Link
                    to="/books/$bookId"
                    params={{ bookId: book.id }}
                    className="font-medium hover:underline"
                  >
                    {book.title}
                  </Link>
                </TableCell>
                <TableCell>{book.author}</TableCell>
                <TableCell>
                  {book.currentPage} / {book.totalPages}
                </TableCell>
                <TableCell>
                  <BookStatusBadge status={book.status} />
                </TableCell>
                <TableCell className="space-x-2 text-right">
                  <Button
                    size="sm"
                    variant="outline"
                    onClick={() =>
                      navigate({
                        to: "/books/edit/$bookId",
                        params: { bookId: book.id },
                      })
                    }
                  >
                    Edit
                  </Button>
                  <ButtonWithLoading
                    size="sm"
                    variant="destructive"
                    isLoading={deleteBookMutation.isPending}
                    onClick={() => deleteBookMutation.mutate(book.id)}
                  >
                    Delete
                  </ButtonWithLoading>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </>
  );
}

export default BooksTable;
