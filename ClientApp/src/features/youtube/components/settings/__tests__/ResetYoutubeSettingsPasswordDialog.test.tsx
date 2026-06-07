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
  useResetYoutubeSettingsPasswordViaEmailMutation: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
}));

import ResetYoutubeSettingsPasswordDialog from "../ResetYoutubeSettingsPasswordDialog";

describe("ResetYoutubeSettingsPasswordDialog", () => {
  it("sends reset email and closes", async () => {
    mockMutateAsync.mockResolvedValue(undefined);
    const onOpenChange = vi.fn();

    render(
      <ResetYoutubeSettingsPasswordDialog open onOpenChange={onOpenChange} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Send email/i }));

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalled();
      expect(onOpenChange).toHaveBeenCalledWith(false);
    });
  });
});
