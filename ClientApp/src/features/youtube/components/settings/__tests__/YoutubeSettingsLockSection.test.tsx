import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("../ChangeYoutubeSettingsPasswordDialog", () => ({
  default: () => null,
}));
vi.mock("../RemoveYoutubeSettingsLockDialog", () => ({
  default: () => null,
}));
vi.mock("../GeneratePasswordModal", () => ({
  default: () => null,
}));
vi.mock("../../../mutations/useYoutubeSettingsPasswordMutations", () => ({
  useSetYoutubeSettingsPasswordMutation: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

import YoutubeSettingsLockSection from "../YoutubeSettingsLockSection";

describe("YoutubeSettingsLockSection", () => {
  it("shows setup form when no password is configured", () => {
    render(<YoutubeSettingsLockSection hasSettingsPassword={false} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText("Settings lock")).toBeInTheDocument();
    expect(screen.getByLabelText("New password")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Enable settings lock/i })).toBeInTheDocument();
  });

  it("shows change and remove actions when a password exists", () => {
    render(<YoutubeSettingsLockSection hasSettingsPassword />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByRole("button", { name: /Change password/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Remove lock/i })).toBeInTheDocument();
  });
});
