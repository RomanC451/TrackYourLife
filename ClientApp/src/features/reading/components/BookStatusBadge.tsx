import { Badge } from "@/components/ui/badge";
import { BookStatus } from "@/services/openapi";

const statusLabels: Record<BookStatus, string> = {
  [BookStatus.NotStarted]: "Not Started",
  [BookStatus.Ongoing]: "Ongoing",
  [BookStatus.Finished]: "Finished",
};

const statusVariants: Record<
  BookStatus,
  "default" | "secondary" | "outline"
> = {
  [BookStatus.NotStarted]: "outline",
  [BookStatus.Ongoing]: "default",
  [BookStatus.Finished]: "secondary",
};

function BookStatusBadge({ status }: { status: BookStatus }) {
  return (
    <Badge variant={statusVariants[status]}>{statusLabels[status]}</Badge>
  );
}

export default BookStatusBadge;
