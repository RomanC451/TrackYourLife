// import "@testing-library/jest-dom";

// import { fireEvent, render, screen } from "@testing-library/react";

// import CustomTextField from "../CustomTextField";

// describe("Text field tests", () => {
//   test("should reder without crashing", () => {
//     render(
//       <CustomTextField
//         label="Label Text"
//         name="name"
//         className=""
//         onChange={() => {}}
//       />
//     );
//     const inputElement = screen.getByRole("textbox");
//     const labelElement = screen.getByText("Label Text", { selector: "label" });
//     expect(inputElement).toBeInTheDocument();
//     expect(labelElement).toBeInTheDocument();
//   });

//   test("The label can be set via label prop", () => {
//     render(
//       <CustomTextField
//         label="This Label Text"
//         name="name"
//         className=""
//         onChange={() => {}}
//       />
//     );
//     const labelElement = screen.getByText("This Label Text", {
//       selector: "label"
//     });

//     expect(labelElement).toBeInTheDocument();
//   });

//   test("The text box should be empty after first render", () => {
//     render(
//       <CustomTextField
//         label="Label Text"
//         name="name"
//         className=""
//         onChange={() => {}}
//       />
//     );
//     const inputElement = screen.getByRole("textbox") as HTMLInputElement;
//     expect(inputElement.value).toBe("");
//   });

//   test("The value of the text box can be changed", () => {
//     render(
//       <CustomTextField
//         label="Label Text"
//         name="name"
//         className=""
//         onChange={() => {}}
//       />
//     );
//     const inputElement = screen.getByRole("textbox") as HTMLInputElement;
//     fireEvent.change(inputElement, {
//       target: { value: "The text was changed" }
//     });
//     expect(inputElement.value).toBe("The text was changed");
//   });

//   test("OnChange function should be called", () => {
//     const mockedOnChange = jest.fn();
//     render(
//       <CustomTextField
//         label="Label Text"
//         name="name"
//         className=""
//         onChange={mockedOnChange}
//       />
//     );
//     const inputElement = screen.getByRole("textbox");
//     fireEvent.change(inputElement, {
//       target: { value: "The text was changed" }
//     });
//     fireEvent.change(inputElement, {
//       target: { value: "The text was changed2" }
//     });
//     expect(mockedOnChange).toHaveBeenCalledTimes(2);
//   });
// });
