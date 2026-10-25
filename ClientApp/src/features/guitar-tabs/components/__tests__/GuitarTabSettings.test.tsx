import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it } from "vitest";

import {
  getGuitarTabPlaybackSettings,
  resetGuitarTabPlaybackSettings,
} from "../../lib/playbackSettings";
import GuitarTabSettings from "../GuitarTabSettings";

describe("GuitarTabSettings", () => {
  beforeEach(() => {
    resetGuitarTabPlaybackSettings();
  });

  it("turns space control of autoscroll on and off", () => {
    render(<GuitarTabSettings />);

    const autoscroll = screen.getByRole("checkbox", { name: "Autoscroll" });
    expect(autoscroll).toHaveAttribute("data-state", "checked");
    expect(screen.queryByRole("checkbox", { name: "Sync audio" })).not.toBeInTheDocument();
    expect(screen.queryByRole("checkbox", { name: "Sync video" })).not.toBeInTheDocument();

    fireEvent.click(autoscroll);

    expect(getGuitarTabPlaybackSettings()).toEqual({
      autoscroll: false,
      syncVideo: true,
    });
    expect(autoscroll).toHaveAttribute("data-state", "unchecked");
  });
});
