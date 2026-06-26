"use no memo";

import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { flexRender, Table as TableT } from "@tanstack/react-table";

import PageTitle from "@/components/common/PageTitle";
import { Button } from "@/components/ui/button";
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
import { BookDto, BookStatus } from "@/services/openapi";

import useBooksTable from "./booksTable/useBooksTable";
import { booksQueryOptions } from "../queries/booksQuery";

function BooksTable() {
  const { data } = useSuspenseQuery(booksQueryOptions.list());
  const { table } = useBooksTable(data);

  if (data.length === 0) {
    return <BooksTable.Empty />;
  }

  return (
    <>
      <BooksTable.Header table={table} />
      <BooksTable.Content table={table} />
    </>
  );
}

type BooksTableHeaderProps = {
  table: TableT<BookDto>;
};

BooksTable.Header = function Header({ table }: BooksTableHeaderProps) {
  const statusFilter =
    (table.getColumn("status")?.getFilterValue() as BookStatus | undefined) ??
    "all";

  return (
    <>
      <PageTitle title="Books">
        <Button asChild>
          <Link to="/reading/books/create">Add book</Link>
        </Button>
      </PageTitle>

      <div className="mb-4 flex flex-wrap gap-3">
        <Select
          value={statusFilter}
          onValueChange={(value) =>
            table
              .getColumn("status")
              ?.setFilterValue(value === "all" ? undefined : value)
          }
        >
          <SelectTrigger className="w-40">
            <SelectValue placeholder="Filter status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            {Object.values(BookStatus).map((status) => (
              <SelectItem key={status} value={status}>
                {status}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
    </>
  );
};

type BooksTableContentProps = {
  table: TableT<BookDto>;
};

BooksTable.Content = function Content({ table }: BooksTableContentProps) {
  const rows = table.getRowModel().rows;

  if (rows.length === 0) {
    return (
      <p className="text-muted-foreground">No books match the selected filter.</p>
    );
  }

  return (
    <Table>
      <TableHeader>
        {table.getHeaderGroups().map((headerGroup) => (
          <TableRow key={headerGroup.id}>
            {headerGroup.headers.map((header) => (
              <TableHead key={header.id}>
                {header.isPlaceholder
                  ? null
                  : flexRender(
                      header.column.columnDef.header,
                      header.getContext(),
                    )}
              </TableHead>
            ))}
          </TableRow>
        ))}
      </TableHeader>
      <TableBody>
        {rows.map((row) => (
          <TableRow key={row.id}>
            {row.getVisibleCells().map((cell) => (
              <TableCell key={cell.id}>
                {flexRender(cell.column.columnDef.cell, cell.getContext())}
              </TableCell>
            ))}
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

BooksTable.Empty = function Empty() {
  return (
    <>
      <PageTitle title="Books">
        <Button asChild>
          <Link to="/reading/books/create">Add book</Link>
        </Button>
      </PageTitle>
      <p className="text-muted-foreground">No books yet. Add your first book.</p>
    </>
  );
};

export default BooksTable;
