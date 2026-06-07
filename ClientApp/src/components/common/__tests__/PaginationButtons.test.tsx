import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { PaginationButtons } from "../PaginationButtons";

const defaultPagedData = {
  page: 2,
  maxPage: 5,
  hasPreviousPage: true,
  hasNextPage: true,
};

describe("PaginationButtons", () => {
  it("renders nothing when pagination should be hidden", () => {
    const { container } = render(
      <PaginationButtons
        pagedData={{ page: 1, maxPage: 1, hasPreviousPage: false, hasNextPage: false }}
        onPreviousPage={vi.fn()}
        onNextPage={vi.fn()}
      />,
    );

    expect(container).toBeEmptyDOMElement();
  });

  it("renders a layout placeholder when maintainLayout is true", () => {
    const { container } = render(
      <PaginationButtons
        pagedData={{ page: 1, maxPage: 1, hasPreviousPage: false, hasNextPage: false }}
        onPreviousPage={vi.fn()}
        onNextPage={vi.fn()}
        maintainLayout
      />,
    );

    expect(container.querySelector("[aria-hidden='true']")).toBeInTheDocument();
  });

  it("renders page links and highlights the active page", () => {
    render(
      <PaginationButtons
        pagedData={defaultPagedData}
        onPreviousPage={vi.fn()}
        onNextPage={vi.fn()}
      />,
    );

    expect(screen.getByRole("link", { name: "2" })).toHaveAttribute(
      "aria-current",
      "page",
    );
    expect(screen.getByRole("link", { name: "1" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "5" })).toBeInTheDocument();
  });

  it("calls navigation handlers when controls are clicked", () => {
    const onPreviousPage = vi.fn();
    const onNextPage = vi.fn();
    const onPageChange = vi.fn();

    render(
      <PaginationButtons
        pagedData={defaultPagedData}
        onPreviousPage={onPreviousPage}
        onNextPage={onNextPage}
        onPageChange={onPageChange}
      />,
    );

    fireEvent.click(screen.getByRole("link", { name: /previous/i }));
    fireEvent.click(screen.getByRole("link", { name: /next/i }));
    fireEvent.click(screen.getByRole("link", { name: "4" }));

    expect(onPreviousPage).toHaveBeenCalledTimes(1);
    expect(onNextPage).toHaveBeenCalledTimes(1);
    expect(onPageChange).toHaveBeenCalledWith(4);
  });

  it("does not call handlers when disabled", () => {
    const onPreviousPage = vi.fn();
    const onNextPage = vi.fn();
    const onPageChange = vi.fn();

    render(
      <PaginationButtons
        pagedData={defaultPagedData}
        onPreviousPage={onPreviousPage}
        onNextPage={onNextPage}
        onPageChange={onPageChange}
        disabled
      />,
    );

    fireEvent.click(screen.getByRole("link", { name: /previous/i }));
    fireEvent.click(screen.getByRole("link", { name: /next/i }));
    fireEvent.click(screen.getByRole("link", { name: "4" }));

    expect(onPreviousPage).not.toHaveBeenCalled();
    expect(onNextPage).not.toHaveBeenCalled();
    expect(onPageChange).not.toHaveBeenCalled();
  });

  it("uses a custom showCondition", () => {
    render(
      <PaginationButtons
        pagedData={{ page: 1, maxPage: 3, hasPreviousPage: false, hasNextPage: true }}
        onPreviousPage={vi.fn()}
        onNextPage={vi.fn()}
        showCondition={(data) => data.maxPage > 2}
      />,
    );

    expect(screen.getByRole("navigation", { name: "pagination" })).toBeInTheDocument();
  });
});
