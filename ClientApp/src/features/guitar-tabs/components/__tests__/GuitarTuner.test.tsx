import { act, fireEvent, render, screen } from "@testing-library/react";
import type { ReactElement } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { frequencyFromMidi } from "../../lib/tuningPresets";

const microphone = vi.hoisted(() => ({
  status: "listening" as "listening" | "denied" | "requesting" | "insecure",
  pitchHz: null as number | null,
  pitchFrame: 0,
  request: vi.fn(),
}));

const pluck = vi.hoisted(() => ({
  play: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("../../hooks/useTunerMicrophone", async () => {
  const actual = await vi.importActual<
    typeof import("../../hooks/useTunerMicrophone")
  >("../../hooks/useTunerMicrophone");
  return {
    ...actual,
    useTunerMicrophone: () => microphone,
  };
});

vi.mock("../../lib/referencePluck", () => ({
  playReferencePluck: () => pluck.play(),
}));

function finishedPluck() {
  return { stop: vi.fn(), finished: Promise.resolve() };
}

function hear(
  rerender: (ui: ReactElement) => void,
  hz: number | null,
  frames = 3,
) {
  for (let i = 0; i < frames; i++) {
    microphone.pitchHz = hz;
    microphone.pitchFrame += 1;
    rerender(<GuitarTuner />);
  }
}

async function flushPluckStart() {
  await act(async () => {
    await Promise.resolve();
  });
}

import { resetTunerLibrary } from "../../lib/customTunings";
import GuitarTuner from "../GuitarTuner";

describe("GuitarTuner", () => {
  beforeEach(() => {
    resetTunerLibrary();
    microphone.status = "listening";
    microphone.pitchHz = null;
    microphone.pitchFrame = 0;
    microphone.request.mockReset();
    pluck.play.mockReset();
    pluck.play.mockImplementation(() => finishedPluck());
    class FakeAudioContext {
      state = "running";
      resume() {
        return Promise.resolve();
      }
      close() {
        return Promise.resolve();
      }
    }
    vi.stubGlobal("AudioContext", FakeAudioContext);
  });

  it("opens on Standard in nearest mode", () => {
    render(<GuitarTuner />);

    expect(screen.getByRole("combobox")).toHaveTextContent("Standard");
    expect(screen.queryByRole("button", { name: "Edit" })).not.toBeInTheDocument();
    expect(screen.getAllByRole("button", { name: "E, string 1" })).toHaveLength(1);
    expect(screen.getByRole("button", { name: "A, string 2" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Auto" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
    expect(screen.getByTestId("tuner-note")).toHaveTextContent("—");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("");
  });

  it("shows the nearest string and cents from the microphone", () => {
    const view = render(<GuitarTuner />);
    hear(view.rerender, frequencyFromMidi(45) * 2 ** (7 / 1200));

    expect(screen.getByTestId("tuner-note")).toHaveTextContent("A");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("+5");
    expect(screen.getByRole("button", { name: "A, string 2" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });

  it("reads a low E heard an octave high as the low E", () => {
    const view = render(<GuitarTuner />);
    hear(view.rerender, frequencyFromMidi(40) * 2);

    expect(screen.getByTestId("tuner-note")).toHaveTextContent("E");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("0");
    expect(screen.getByRole("button", { name: "E, string 1" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
    expect(screen.getByRole("button", { name: "E, string 6" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("ignores a one-frame jump to a far cents value", async () => {
    const view = render(<GuitarTuner />);
    fireEvent.click(screen.getByRole("button", { name: "E, string 1" }));
    await flushPluckStart();

    const lowE = frequencyFromMidi(40);
    hear(view.rerender, lowE);
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("0");

    hear(view.rerender, lowE * 2 ** (-150 / 1200), 1);
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("0");

    hear(view.rerender, lowE * 2 ** (-150 / 1200));
    expect(screen.getByTestId("tuner-note")).toHaveTextContent("E");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("-150");
  });

  it("locks a string, replays it, and returns to nearest mode", async () => {
    render(<GuitarTuner />);

    const lowE = screen.getByRole("button", { name: "E, string 1" });
    fireEvent.click(lowE);
    await flushPluckStart();
    fireEvent.click(lowE);
    await flushPluckStart();

    expect(pluck.play).toHaveBeenCalledTimes(2);
    expect(lowE).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByRole("button", { name: "Auto" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );

    fireEvent.click(screen.getByRole("button", { name: "Auto" }));
    expect(lowE).toHaveAttribute("aria-pressed", "false");
    expect(screen.getByRole("button", { name: "Auto" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });

  it("keeps the locked string when the tuning changes", async () => {
    render(<GuitarTuner />);

    fireEvent.click(screen.getByRole("button", { name: "E, string 1" }));
    await flushPluckStart();
    fireEvent.click(screen.getByRole("combobox"));
    fireEvent.click(screen.getByRole("option", { name: "Drop D" }));

    expect(screen.getByRole("combobox")).toHaveTextContent("Drop D");
    const lowString = screen.getByRole("button", { name: "D, string 1" });
    expect(lowString).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByRole("button", { name: "Auto" })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
  });

  it("idles the meter while the reference pluck is playing", async () => {
    let finishPluck = () => {};
    pluck.play.mockImplementation(() => ({
      stop: vi.fn(),
      finished: new Promise<void>((resolve) => {
        finishPluck = resolve;
      }),
    }));
    const view = render(<GuitarTuner />);
    hear(view.rerender, frequencyFromMidi(45));

    fireEvent.click(screen.getByRole("button", { name: "A, string 2" }));
    await flushPluckStart();

    expect(screen.getByTestId("tuner-note")).toHaveTextContent("—");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("");
    expect(screen.getByRole("button", { name: "A, string 2" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );

    await act(async () => {
      finishPluck();
    });
    hear(view.rerender, frequencyFromMidi(45));
    expect(screen.getByTestId("tuner-note")).toHaveTextContent("A");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("0");
  });

  it("keeps plucks available when the microphone is denied", async () => {
    microphone.status = "denied";
    render(<GuitarTuner />);

    expect(screen.getByText(/blocked the microphone/i)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Allow microphone" }));
    expect(microphone.request).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByRole("button", { name: "E, string 1" }));
    await flushPluckStart();
    expect(pluck.play).toHaveBeenCalledTimes(1);
  });

  it("sends an insecure address to localhost instead of a dead microphone button", () => {
    microphone.status = "insecure";
    render(<GuitarTuner />);

    expect(
      screen.queryByRole("button", { name: "Allow microphone" }),
    ).not.toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Open on localhost" })).toHaveAttribute(
      "href",
      expect.stringMatching(/^http:\/\/127\.0\.0\.1(:\d+)?\//),
    );
  });

  it("saves a custom tuning and opens on it next time", () => {
    const first = render(<GuitarTuner />);
    fireEvent.click(screen.getByRole("button", { name: "New tuning" }));
    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "C# Tuning" },
    });
    fireEvent.change(screen.getByLabelText("Notes"), {
      target: { value: "C# F# B E G# C#" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save tuning" }));

    expect(screen.getByRole("combobox")).toHaveTextContent("C# Tuning");
    expect(screen.getByRole("button", { name: "C#, string 1" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "F#, string 2" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "C#, string 6" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Edit" })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Remove" })).toBeInTheDocument();

    first.unmount();
    render(<GuitarTuner />);

    expect(screen.getByRole("combobox")).toHaveTextContent("C# Tuning");
    expect(screen.getByRole("button", { name: "B, string 3" })).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Remove" }));
    expect(screen.getByRole("combobox")).toHaveTextContent("Standard");
    expect(screen.queryByRole("button", { name: "Remove" })).not.toBeInTheDocument();
  });

  it("edits the name and notes of the selected custom tuning", () => {
    render(<GuitarTuner />);
    fireEvent.click(screen.getByRole("button", { name: "New tuning" }));
    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "C# Tuning" },
    });
    fireEvent.change(screen.getByLabelText("Notes"), {
      target: { value: "C# F# B E G# C#" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save tuning" }));

    fireEvent.click(screen.getByRole("button", { name: "Edit" }));
    expect(screen.getByRole("dialog", { name: "Edit tuning" })).toBeInTheDocument();
    expect(screen.getByLabelText("Name")).toHaveValue("C# Tuning");
    expect(screen.getByLabelText("Notes")).toHaveValue("C# F# B E G# C#");

    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "Open G" },
    });
    fireEvent.change(screen.getByLabelText("Notes"), {
      target: { value: "D G D G B D" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save tuning" }));

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByRole("combobox")).toHaveTextContent("Open G");
    expect(screen.getByRole("button", { name: "D, string 1" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "G, string 2" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "D, string 6" })).toBeInTheDocument();
  });

  it("keeps the dialog open when the notes are not six pitches", () => {
    render(<GuitarTuner />);
    fireEvent.click(screen.getByRole("button", { name: "New tuning" }));
    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "C# Tuning" },
    });
    fireEvent.change(screen.getByLabelText("Notes"), {
      target: { value: "C# F# B" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Save tuning" }));

    expect(screen.getByRole("dialog", { name: "New tuning" })).toBeInTheDocument();
    expect(
      screen.getByText("Enter six notes, from the low string to the high string."),
    ).toBeInTheDocument();
    expect(screen.getByRole("combobox", { hidden: true })).toHaveTextContent(
      "Standard",
    );
  });
});
