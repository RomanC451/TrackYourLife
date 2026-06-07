import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import NavBarLayout from "../NavBarLayout";

describe("NavBarLayout", () => {
  it("renders the navbar and page content", () => {
    render(
      <NavBarLayout navBarElement={<nav>Navbar</nav>}>
        <main>Content</main>
      </NavBarLayout>,
    );

    expect(screen.getByText("Navbar")).toBeInTheDocument();
    expect(screen.getByText("Content")).toBeInTheDocument();
  });
});
