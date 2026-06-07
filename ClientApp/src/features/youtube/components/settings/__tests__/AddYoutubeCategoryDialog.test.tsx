import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("../../../mutations/useYoutubeCategoryMutations", () => ({
  useCreateYoutubeCategoryMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

import { AddYoutubeCategoryDialog } from "../AddYoutubeCategoryDialog";

describe("AddYoutubeCategoryDialog", () => {
  it("creates a category", () => {
    render(<AddYoutubeCategoryDialog />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByRole("button", { name: /Add category/i }));
    fireEvent.change(screen.getByLabelText("Name"), {
      target: { value: "Learning" },
    });
    fireEvent.change(screen.getByLabelText(/Max \/ day/i), {
      target: { value: "3" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Create$/i }));

    expect(mockMutate).toHaveBeenCalledWith(
      { name: "Learning", maxVideosPerDay: 3 },
      expect.any(Object),
    );
  });
});
