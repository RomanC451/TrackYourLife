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
  useCreatePlaylistMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import CreatePlaylistDialog from "../CreatePlaylistDialog";

describe("CreatePlaylistDialog", () => {
  it("creates a playlist", () => {
    mockMutate.mockImplementation((_name, options) => {
      options?.onSuccess?.({ id: "pl-1" });
    });
    const onOpenChange = vi.fn();
    const onCreated = vi.fn();

    render(
      <CreatePlaylistDialog open onOpenChange={onOpenChange} onCreated={onCreated} />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "Watch later" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Create/i }));

    expect(mockMutate).toHaveBeenCalledWith("Watch later", expect.any(Object));
    expect(onCreated).toHaveBeenCalledWith("pl-1");
  });
});
