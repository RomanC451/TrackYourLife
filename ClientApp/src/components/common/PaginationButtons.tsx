import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { cn } from "@/lib/utils";

interface PaginationData {
  page: number;
  maxPage: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

interface PaginationButtonsProps {
  pagedData: PaginationData | null | undefined;
  onPreviousPage: () => void;
  onNextPage: () => void;
  onPageChange?: (page: number) => void;
  /**
   * Optional condition to show pagination. If not provided, defaults to showing
   * when maxPage > 1 or page > 1
   */
  showCondition?: (pagedData: PaginationData) => boolean;
  /**
   * If true, renders a placeholder div to maintain layout when pagination is hidden
   */
  maintainLayout?: boolean;
  /**
   * When true, all pagination controls are disabled (e.g. during navigation).
   */
  disabled?: boolean;
  /**
   * When true, all pagination controls are disabled and show loading state (e.g. when query is fetching).
   */
  loading?: boolean;
}

/**
 * Generates an array of page numbers to display, with ellipsis where needed
 */
function generatePageNumbers(currentPage: number, maxPage: number): (number | "ellipsis")[] {
  const pages: (number | "ellipsis")[] = [];
  const delta = 1; // Number of pages to show on each side of current page

  if (maxPage <= 7) {
    // Show all pages if 7 or fewer
    for (let i = 1; i <= maxPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  // Always show first page
  pages.push(1);

  let startPage = Math.max(2, currentPage - delta);
  let endPage = Math.min(maxPage - 1, currentPage + delta);

  // Adjust if we're near the start
  if (currentPage <= 3) {
    endPage = Math.min(5, maxPage - 1);
  }

  // Adjust if we're near the end
  if (currentPage >= maxPage - 2) {
    startPage = Math.max(2, maxPage - 4);
  }

  // Add ellipsis after first page if needed
  if (startPage > 2) {
    pages.push("ellipsis");
  }

  // Add middle pages
  for (let i = startPage; i <= endPage; i++) {
    pages.push(i);
  }

  // Add ellipsis before last page if needed
  if (endPage < maxPage - 1) {
    pages.push("ellipsis");
  }

  // Always show last page
  if (maxPage > 1) {
    pages.push(maxPage);
  }

  return pages;
}

export function PaginationButtons({
  pagedData,
  onPreviousPage,
  onNextPage,
  onPageChange,
  showCondition,
  maintainLayout = false,
  disabled = false,
  loading = false,
}: PaginationButtonsProps) {
  const isDisabled = disabled || loading;
  const shouldShow =
    pagedData &&
    (showCondition
      ? showCondition(pagedData)
      : pagedData.maxPage > 1 || pagedData.page > 1);

  if (!shouldShow) {
    if (maintainLayout) {
      return (
        <div className="flex items-center gap-2" aria-hidden="true">
          {/* Placeholder to maintain layout when pagination is not shown */}
        </div>
      );
    }
    return null;
  }

  const pageNumbers = generatePageNumbers(pagedData.page, pagedData.maxPage);

  const handlePageClick = (page: number, e: React.MouseEvent<HTMLAnchorElement>) => {
    e.preventDefault();
    if (!isDisabled && onPageChange && page !== pagedData.page) {
      onPageChange(page);
    }
  };

  const handlePreviousClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    e.preventDefault();
    if (!isDisabled && pagedData.hasPreviousPage) {
      onPreviousPage();
    }
  };

  const handleNextClick = (e: React.MouseEvent<HTMLAnchorElement>) => {
    e.preventDefault();
    if (!isDisabled && pagedData.hasNextPage) {
      onNextPage();
    }
  };

  const hasPrevious = pagedData.hasPreviousPage;
  const hasNext = pagedData.hasNextPage;

  return (
    <Pagination aria-busy={loading}>
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            href="#"
            onClick={handlePreviousClick}
            className={cn(
              hasPrevious && !isDisabled
                ? "cursor-pointer"
                : "pointer-events-none opacity-50",
            )}
            aria-disabled={isDisabled || !hasPrevious}
          />
        </PaginationItem>
        {pageNumbers.map((page, index) => {
          if (page === "ellipsis") {
            // Use position-based key: first ellipsis appears after page 1, second before last page
            const isFirstEllipsis = index === 1;
            return (
              <PaginationItem key={isFirstEllipsis ? "ellipsis-start" : "ellipsis-end"}>
                <PaginationEllipsis />
              </PaginationItem>
            );
          }
          return (
            <PaginationItem key={`page-${page}`}>
              <PaginationLink
                href="#"
                onClick={(e) => handlePageClick(page, e)}
                isActive={page === pagedData.page}
                className={cn(
                  isDisabled ? "pointer-events-none opacity-50" : "cursor-pointer",
                )}
                aria-disabled={isDisabled}
              >
                {page}
              </PaginationLink>
            </PaginationItem>
          );
        })}
        <PaginationItem>
          <PaginationNext
            href="#"
            onClick={handleNextClick}
            className={cn(
              hasNext && !isDisabled
                ? "cursor-pointer"
                : "pointer-events-none opacity-50",
            )}
            aria-disabled={isDisabled || !hasNext}
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}
