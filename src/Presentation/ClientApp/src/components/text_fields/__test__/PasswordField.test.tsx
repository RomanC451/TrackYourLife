import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import PasswordField from "../PasswordField";

const mockedOnChange = jest.fn();

describe("Text field tests", () => {
  test("should reder without crashing", () => {
    render(
      <PasswordField
        label="Label Text"
        name="labelText"
        id={1}
        className=""
        onChange={() => {}}
      />
    );
    const inputElement = screen.getByTestId("id-input-1");
    const labelElement = screen.getByText("Label Text", { selector: "label" });
    const buttonElement = screen.getByRole("button");

    expect(inputElement).toBeInTheDocument();
    expect(labelElement).toBeInTheDocument();
    expect(buttonElement).toBeInTheDocument();
  });

  test("The label can be set via label prop", () => {
    render(
      <PasswordField
        label="This Label Text"
        name="labelText"
        id={1}
        className=""
        onChange={() => {}}
      />
    );
    const labelElement = screen.getByText("This Label Text", {
      selector: "label",
    });

    expect(labelElement).toBeInTheDocument();
  });

  test("The text box should be empty after first render", () => {
    render(
      <PasswordField
        label="Label Text"
        name="labelText"
        id={1}
        className=""
        onChange={() => {}}
      />
    );
    const inputElement = screen.getByTestId("id-input-1");
    // @ts-ignore
    expect(inputElement.value).toBe("");
  });

  test("The value of the text box can be changed", () => {
    render(
      <PasswordField
        label="Label Text"
        name="labelText"
        id={1}
        className=""
        onChange={() => {}}
      />
    );
    const inputElement = screen.getByTestId("id-input-1") as HTMLInputElement;
    fireEvent.change(inputElement, {
      target: { value: "The text has changed" },
    });

    expect(inputElement.value).toBe("The text has changed");
  });

  test("OnChange function should be called", () => {
    render(
      <PasswordField
        label="Label Text"
        name="labelText"
        onChange={mockedOnChange}
        id={1}
        className=""
      />
    );
    const inputElement = screen.getByTestId("id-input-1");
    fireEvent.change(inputElement, {
      target: { value: "The text has changed" },
    });
    fireEvent.change(inputElement, {
      target: { value: "The text has changed2" },
    });
    expect(mockedOnChange).toHaveBeenCalledTimes(2);
  });

  test("Toggle password visibility button should hide the password", () => {
    render(
      <PasswordField
        label="Label Text"
        name="labelText"
        onChange={mockedOnChange}
        id={1}
        className=""
      />
    );
    const inputElement = screen.getByTestId("id-input-1");
    const buttonElement = screen.getByRole("button");

    // @ts-ignore
    expect(inputElement.type).toBe("password");
    fireEvent.click(buttonElement);
    // @ts-ignore
    expect(inputElement.type).toBe("text");
  });
});
