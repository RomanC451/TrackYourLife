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

  it("turns space control of autoscroll and the video on and off", () => {
    render(<GuitarTabSettings />);

    const autoscroll = screen.getByRole("checkbox", { name: "Autoscroll" });
    const video = screen.getByRole("checkbox", { name: "Sync video" });
    expect(autoscroll).toHaveAttribute("data-state", "checked");
    expect(video).toHaveAttribute("data-state", "checked");

    fireEvent.click(autoscroll);
    fireEvent.click(video);

    expect(getGuitarTabPlaybackSettings()).toEqual({
      autoscroll: false,
      syncVideo: false,
    });
    expect(autoscroll).toHaveAttribute("data-state", "unchecked");
    expect(video).toHaveAttribute("data-state", "unchecked");
  });
});
