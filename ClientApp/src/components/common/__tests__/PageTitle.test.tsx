import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import PageTitle from "../PageTitle";

describe("PageTitle", () => {
  it("renders the title", () => {
    render(<PageTitle title="Workouts" />);

    expect(screen.getByText("Workouts")).toBeInTheDocument();
  });

  it("renders optional action content", () => {
    render(
      <PageTitle title="Workouts">
        <button type="button">Add</button>
      </PageTitle>,
    );

    expect(screen.getByRole("button", { name: "Add" })).toBeInTheDocument();
  });
});
