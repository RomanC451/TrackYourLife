import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useLibraryPlaylistMutations", () => ({
  useUpdatePlaylistMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import EditPlaylistDialog from "../EditPlaylistDialog";

describe("EditPlaylistDialog", () => {
  it("renames a playlist", () => {
    const onOpenChange = vi.fn();

    render(
      <EditPlaylistDialog
        open
        onOpenChange={onOpenChange}
        playlistId="pl-1"
        initialName="Old name"
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "New name" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Save/i }));

    expect(mockMutate).toHaveBeenCalledWith(
      { playlistId: "pl-1", name: "New name" },
      expect.any(Object),
    );
  });
});
