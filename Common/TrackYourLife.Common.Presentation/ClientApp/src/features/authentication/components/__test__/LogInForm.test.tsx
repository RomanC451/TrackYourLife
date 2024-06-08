// import "@testing-library/jest-dom";
// import { cleanup, render, screen } from "@testing-library/react";

// import LogInForm from "../LogInForm.1";

// var authMode = "login";

// function MockSwitchAuthMode() {
//   authMode = authMode === "login" ? "signup" : "login";
// }

// describe("Login form tests", () => {
//   describe("Component rendering", () => {
//     afterEach(cleanup);
//     test("Title div", () => {
//       const container = document.createElement("div");
//       document.body.appendChild(container);
//       render(<LogInForm />, {
//         container: container,
//       });
//       const titleDiv = screen.getByText("LOG IN");
//       expect(titleDiv).toBeInTheDocument();
//     });
//     test("Title div", () => {
//       render(<LogInForm />);
//       const titleDiv = screen.getByText("Take control of your life");
//       expect(titleDiv).toBeInTheDocument();
//     });
//     test("Email input", () => {
//       render(<LogInForm />);
//       const inputElement = screen.getByText("Email", { selector: "label" });
//       expect(inputElement).toBeInTheDocument();
//     });
//     test("Password input", () => {
//       render(<LogInForm />);
//       const inputElement = screen.getByText("Password", { selector: "label" });
//       expect(inputElement).toBeInTheDocument();
//     });
//     test("Login button", () => {
//       render(<LogInForm />);
//       const buttonElement = screen.getByRole("button", {
//         name: "I don't have an account.",
//       });

//       expect(buttonElement).toBeInTheDocument();
//     });
//   });
// });
