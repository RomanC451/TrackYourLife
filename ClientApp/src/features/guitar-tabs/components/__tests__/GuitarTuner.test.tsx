import { act, fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { frequencyFromMidi } from "../../lib/tuningPresets";

const microphone = vi.hoisted(() => ({
  status: "listening" as "listening" | "denied" | "requesting" | "insecure",
  pitchHz: null as number | null,
  request: vi.fn(),
}));

const pluck = vi.hoisted(() => ({
  play: vi.fn(),
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

async function flushPluckStart() {
  await act(async () => {
    await Promise.resolve();
  });
}

import GuitarTuner from "../GuitarTuner";

describe("GuitarTuner", () => {
  beforeEach(() => {
    microphone.status = "listening";
    microphone.pitchHz = null;
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
    microphone.pitchHz = frequencyFromMidi(45) * 2 ** (7 / 1200);
    render(<GuitarTuner />);

    expect(screen.getByTestId("tuner-note")).toHaveTextContent("A");
    expect(screen.getByTestId("tuner-cents")).toHaveTextContent("+7");
    expect(screen.getByRole("button", { name: "A, string 2" })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });

  it("reads a low E heard an octave high as the low E", () => {
    microphone.pitchHz = frequencyFromMidi(40) * 2;
    render(<GuitarTuner />);

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
    microphone.pitchHz = frequencyFromMidi(45);
    render(<GuitarTuner />);

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
});
