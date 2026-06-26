"use no memo";

import { ColumnDef } from "@tanstack/react-table";
import { Link } from "@tanstack/react-router";
import { ArrowDown, ArrowUp, ArrowUpDown } from "lucide-react";

import { Button } from "@/components/ui/button";
import BookStatusBadge from "@/features/reading/components/BookStatusBadge";
import { BookDto } from "@/services/openapi";

import BooksTableRowActions from "./BooksTableRowActions";

// eslint-disable-next-line react-refresh/only-export-components
function SortIcon({ isSorted }: { isSorted: false | "asc" | "desc" }) {
  if (isSorted === false) return <ArrowUpDown />;
  if (isSorted === "asc") return <ArrowUp className="text-primary" />;
  if (isSorted === "desc") return <ArrowDown className="text-primary" />;
}

export const booksTableColumns: ColumnDef<BookDto>[] = [
  {
    id: "actions",
    header: () => (
      <Button variant="ghost" className="w-8" disabled>
        <div className="h-4 w-4" />
      </Button>
    ),
    cell: ({ row }) => <BooksTableRowActions book={row.original} />,
    enableSorting: false,
  },
  {
    accessorKey: "title",
    header: ({ column }) => (
      <Button
        className="p-2"
        variant="ghost"
        onClick={() => column.toggleSorting()}
      >
        Title
        <SortIcon isSorted={column.getIsSorted()} />
      </Button>
    ),
    cell: ({ row }) => (
      <Link
        to="/reading/books/$bookId"
        params={{ bookId: row.original.id }}
        className="font-medium hover:underline"
      >
        {row.original.title}
      </Link>
    ),
    enableSorting: true,
  },
  {
    accessorKey: "author",
    header: ({ column }) => (
      <Button
        className="p-2"
        variant="ghost"
        onClick={() => column.toggleSorting()}
      >
        Author
        <SortIcon isSorted={column.getIsSorted()} />
      </Button>
    ),
    cell: ({ row }) => row.original.author,
    enableSorting: true,
  },
  {
    id: "progress",
    header: ({ column }) => (
      <Button
        className="p-2"
        variant="ghost"
        onClick={() => column.toggleSorting()}
      >
        Progress
        <SortIcon isSorted={column.getIsSorted()} />
      </Button>
    ),
    cell: ({ row }) =>
      `${row.original.currentPage} / ${row.original.totalPages}`,
    accessorFn: (row) => row.currentPage,
    enableSorting: true,
  },
  {
    accessorKey: "status",
    header: () => <p className="p-2">Status</p>,
    cell: ({ row }) => <BookStatusBadge status={row.original.status} />,
    filterFn: (row, id, value) => row.getValue(id) === value,
    enableSorting: false,
  },
];
