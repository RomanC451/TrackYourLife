import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMutateAsync } = vi.hoisted(() => ({
  mockMutateAsync: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("../GeneratePasswordModal", () => ({
  default: () => null,
}));

vi.mock("../../../mutations/useYoutubeSettingsPasswordMutations", () => ({
  useSetYoutubeSettingsPasswordMutation: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
}));

import ChangeYoutubeSettingsPasswordDialog from "../ChangeYoutubeSettingsPasswordDialog";

describe("ChangeYoutubeSettingsPasswordDialog", () => {
  it("changes the settings password", async () => {
    mockMutateAsync.mockResolvedValue(undefined);
    const onOpenChange = vi.fn();

    render(
      <ChangeYoutubeSettingsPasswordDialog open onOpenChange={onOpenChange} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByLabelText("Current password"), {
      target: { value: "OldPass1!" },
    });
    fireEvent.change(screen.getByLabelText("New password"), {
      target: { value: "NewPass1!aa" },
    });
    fireEvent.change(screen.getByLabelText("Confirm new password"), {
      target: { value: "NewPass1!aa" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Update password/i }));

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith({
        currentPassword: "OldPass1!",
        newPassword: "NewPass1!aa",
      });
      expect(onOpenChange).toHaveBeenCalledWith(false);
    });
  });
});
