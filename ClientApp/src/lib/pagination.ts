/**
 * Generates an array of page numbers to display, with ellipsis where needed.
 */
export function generatePageNumbers(
  currentPage: number,
  maxPage: number,
): (number | "ellipsis")[] {
  const pages: (number | "ellipsis")[] = [];
  const delta = 1;

  if (maxPage <= 7) {
    for (let i = 1; i <= maxPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  pages.push(1);

  let startPage = Math.max(2, currentPage - delta);
  let endPage = Math.min(maxPage - 1, currentPage + delta);

  if (currentPage <= 3) {
    endPage = Math.min(5, maxPage - 1);
  }

  if (currentPage >= maxPage - 2) {
    startPage = Math.max(2, maxPage - 4);
  }

  if (startPage > 2) {
    pages.push("ellipsis");
  }

  for (let i = startPage; i <= endPage; i++) {
    pages.push(i);
  }

  if (endPage < maxPage - 1) {
    pages.push("ellipsis");
  }

  if (maxPage > 1) {
    pages.push(maxPage);
  }

  return pages;
}
