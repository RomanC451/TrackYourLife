// import "@testing-library/jest-dom";

// import { fireEvent, render, screen } from "@testing-library/react";
// import { describe, expect, it, vi } from "vitest";

// import ButtonWithLoading from "../button-with-loading";

// describe("ButtonWithLoading", () => {
//   it("renders children when not loading", () => {
//     render(<ButtonWithLoading isLoading={false}>Click me</ButtonWithLoading>);
//     const visibleChild = screen
//       .getAllByText("Click me")
//       .find((el) => el.className !== "opacity-0");
//     expect(visibleChild).toBeInTheDocument();
//   });

//   it("shows loading spinner when loading", () => {
//     render(<ButtonWithLoading isLoading={true}>Click me</ButtonWithLoading>);
//     const visibleChild = screen
//       .getAllByText("Click me")
//       .find((el) => el.className !== "opacity-0");
//     if (visibleChild) {
//       expect(visibleChild).not.toBeInTheDocument();
//     }
//     expect(screen.getByRole("progressbar")).toBeInTheDocument();
//   });

//   it("enables button when not loading", () => {
//     render(<ButtonWithLoading isLoading={false}>Click me</ButtonWithLoading>);
//     const button = screen.getByRole("button");
//     expect(button).toBeEnabled();
//   });

//   it("handles button click event when not loading", () => {
//     const handleClick = vi.fn();
//     render(
//       <ButtonWithLoading isLoading={false} onClick={handleClick}>
//         Click me
//       </ButtonWithLoading>,
//     );
//     const button = screen.getByRole("button");
//     fireEvent.click(button);
//     expect(handleClick).toHaveBeenCalledTimes(1);
//   });
// });
