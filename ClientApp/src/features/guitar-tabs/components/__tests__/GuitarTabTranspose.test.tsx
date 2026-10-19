import { fireEvent, render, screen } from "@testing-library/react";
import { useState } from "react";
import { describe, expect, it } from "vitest";

import GuitarTabTranspose from "../GuitarTabTranspose";

function Harness() {
  const [semitones, setSemitones] = useState(0);
  const [useFlats, setUseFlats] = useState(false);

  return (
    <GuitarTabTranspose
      semitones={semitones}
      useFlats={useFlats}
      onSemitonesChange={setSemitones}
      onUseFlatsChange={setUseFlats}
    />
  );
}

describe("GuitarTabTranspose", () => {
  it("transposes up and down and toggles flats", () => {
    render(<Harness />);

    fireEvent.click(screen.getByRole("button", { name: "Transpose Up" }));
    expect(screen.getByRole("spinbutton", { name: "Transpose" })).toHaveTextContent(
      "Tr. 1",
    );

    fireEvent.click(screen.getByRole("button", { name: "Transpose Down" }));
    fireEvent.click(screen.getByRole("button", { name: "Transpose Down" }));
    expect(screen.getByRole("spinbutton", { name: "Transpose" })).toHaveTextContent(
      "Tr. -1",
    );

    fireEvent.click(screen.getByRole("button", { name: "Use Flats" }));
    expect(screen.getByRole("button", { name: "Use Flats" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });
});