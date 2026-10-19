import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
    queryToolsRef: { current: null },
    routerToolsRef: { current: null },
  }),
}));

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../mutations/useGuitarPlaylistMutations", () => ({
  useCreateGuitarPlaylistMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import CreatePlaylistDialog from "../CreatePlaylistDialog";

describe("CreatePlaylistDialog", () => {
  beforeEach(() => {
    mockMutate.mockReset();
  });
  it("creates a playlist and returns its id", () => {
    mockMutate.mockImplementation((_name, options) => {
      options?.onSuccess?.({ id: "playlist-1" });
    });
    const onOpenChange = vi.fn();
    const onCreated = vi.fn();

    render(
      <CreatePlaylistDialog
        open
        onOpenChange={onOpenChange}
        onCreated={onCreated}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "Fingerstyle" },
    });
    fireEvent.submit(screen.getByRole("button", { name: "Create" }).closest("form")!);

    expect(mockMutate).toHaveBeenCalledWith("Fingerstyle", expect.any(Object));
    expect(onCreated).toHaveBeenCalledWith("playlist-1");
  });

  it("does not create a playlist named Favorites", () => {
    render(<CreatePlaylistDialog open onOpenChange={vi.fn()} />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "Favorites" },
    });

    expect(
      screen.getByText("Favorites is already a page in this section."),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Create" })).toBeDisabled();
    expect(mockMutate).not.toHaveBeenCalled();
  });
});
