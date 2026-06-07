import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockMutateAsync } = vi.hoisted(() => ({
  mockMutateAsync: vi.fn(),
}));

vi.mock("../../../mutations/useYoutubeSettingsPasswordMutations", () => ({
  useSetYoutubeSettingsPasswordMutation: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
}));

import RemoveYoutubeSettingsLockDialog from "../RemoveYoutubeSettingsLockDialog";

describe("RemoveYoutubeSettingsLockDialog", () => {
  it("removes the settings lock", async () => {
    mockMutateAsync.mockResolvedValue(undefined);
    const onOpenChange = vi.fn();

    render(
      <RemoveYoutubeSettingsLockDialog open onOpenChange={onOpenChange} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByLabelText("Current password"), {
      target: { value: "Aa1!aaaaaa" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Remove settings lock/i }));

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith({
        currentPassword: "Aa1!aaaaaa",
      });
    });
  });
});
