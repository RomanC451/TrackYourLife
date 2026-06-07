import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

const { mockMutate } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
}));

vi.mock("@/hooks/useCustomMutation", () => ({
  useCustomMutation: () => ({
    mutate: mockMutate,
    isPending: false,
  }),
}));

vi.mock("@/components/ui/shadcn-io/dropzone", () => ({
  Dropzone: ({
    onDrop,
    children,
  }: {
    onDrop: (files: File[]) => void;
    children: React.ReactNode;
  }) => (
    <div>
      <button
        type="button"
        onClick={() => onDrop([new File(["img"], "photo.png", { type: "image/png" })])}
      >
        Drop file
      </button>
      {children}
    </div>
  ),
  DropzoneContent: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="dropzone-content">{children}</div>
  ),
  DropzoneEmptyState: () => <div data-testid="dropzone-empty" />,
}));

import FileDropZone from "../FileDropZone";

describe("FileDropZone", () => {
  it("uploads a file and calls onSuccess", async () => {
    const onSuccess = vi.fn();
    const uploadFunction = vi.fn().mockResolvedValue("https://cdn.test/photo.png");
    mockMutate.mockImplementation((_file, opts) => {
      opts.onSuccess("https://cdn.test/photo.png");
    });

    render(
      <FileDropZone uploadFunction={uploadFunction} onSuccess={onSuccess} />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Drop file" }));
    await waitFor(() => {
      expect(onSuccess).toHaveBeenCalledWith("https://cdn.test/photo.png");
    });
  });

  it("shows default image when provided", () => {
    render(
      <FileDropZone
        uploadFunction={vi.fn()}
        onSuccess={vi.fn()}
        defaultImageUrl="https://cdn.test/default.png"
      />,
    );

    expect(screen.getByAltText("Preview")).toHaveAttribute(
      "src",
      "https://cdn.test/default.png",
    );
  });
});
