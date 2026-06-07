import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import VirtualizedList from "../virtualized-list";

describe("VirtualizedList", () => {
  const items = Array.from({ length: 100 }, (_, index) => `item-${index}`);

  it("renders only the visible slice of items", () => {
    render(
      <VirtualizedList
        items={items}
        itemHeight={20}
        containerHeight={100}
        overscan={0}
        renderItem={(item) => <div>{item}</div>}
      />,
    );

    expect(screen.getByText("item-0")).toBeInTheDocument();
    expect(screen.getByText("item-4")).toBeInTheDocument();
    expect(screen.queryByText("item-20")).not.toBeInTheDocument();
  });

  it("updates the visible slice when scrolled", () => {
    const { container } = render(
      <VirtualizedList
        items={items}
        itemHeight={20}
        containerHeight={100}
        overscan={0}
        renderItem={(item) => <div>{item}</div>}
      />,
    );

    const scrollContainer = container.firstChild as HTMLDivElement;

    fireEvent.scroll(scrollContainer, { target: { scrollTop: 200 } });

    expect(screen.queryByText("item-0")).not.toBeInTheDocument();
    expect(screen.getByText("item-10")).toBeInTheDocument();
  });
});
