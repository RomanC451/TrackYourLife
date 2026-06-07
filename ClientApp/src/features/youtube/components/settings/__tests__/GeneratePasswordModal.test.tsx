import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("../../../utils/generateYoutubeSettingsPassword", () => ({
  generateYoutubeSettingsPassword: () => "Aa1!generated",
  YOUTUBE_SETTINGS_PASSWORD_HINT: "hint",
}));

import GeneratePasswordModal from "../GeneratePasswordModal";

describe("GeneratePasswordModal", () => {
  beforeEach(() => {
    Object.assign(navigator, {
      clipboard: { writeText: vi.fn().mockResolvedValue(undefined) },
    });
  });

  it("generates and applies a password", () => {
    const onUsePassword = vi.fn();

    render(
      <GeneratePasswordModal
        open
        onOpenChange={vi.fn()}
        onUsePassword={onUsePassword}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Generate/i }));
    expect(screen.getByDisplayValue("Aa1!generated")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("checkbox"));
    fireEvent.click(screen.getByRole("button", { name: /Use this password/i }));

    expect(onUsePassword).toHaveBeenCalledWith("Aa1!generated");
  });
});
