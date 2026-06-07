import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("../ResetYoutubeSettingsPasswordDialog", () => ({
  default: () => null,
}));

import YoutubeSettingsUnlockOverlay from "../YoutubeSettingsUnlockOverlay";

describe("YoutubeSettingsUnlockOverlay", () => {
  it("unlocks settings with a password", async () => {
    const onUnlock = vi.fn().mockResolvedValue(undefined);

    render(<YoutubeSettingsUnlockOverlay onUnlock={onUnlock} isVerifying={false} />);

    fireEvent.change(screen.getByLabelText("Password"), {
      target: { value: "secret" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Unlock settings/i }));

    await waitFor(() => {
      expect(onUnlock).toHaveBeenCalledWith("secret");
    });
  });

  it("shows an error when unlock fails", async () => {
    const onUnlock = vi.fn().mockRejectedValue(new Error("bad password"));

    render(<YoutubeSettingsUnlockOverlay onUnlock={onUnlock} isVerifying={false} />);

    fireEvent.change(screen.getByLabelText("Password"), {
      target: { value: "wrong" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Unlock settings/i }));

    expect(await screen.findByText(/Incorrect password/i)).toBeInTheDocument();
  });
});
