import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { describe, expect, it, vi } from "vitest";

import { youtubeCategory, youtubeSettings } from "@/features/youtube/__tests__/fixtures";
import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";
import {
  youtubeSettingsDtoToForm,
  youtubeSettingsFormSchema,
  type YoutubeSettingsFormSchema,
} from "@/features/youtube/data/youtubeSettingsSchemas";

vi.mock("@dnd-kit/sortable", () => ({
  useSortable: () => ({
    attributes: {},
    listeners: {},
    setNodeRef: vi.fn(),
    transform: null,
    transition: undefined,
    isDragging: false,
  }),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuTrigger: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  DropdownMenuItem: ({
    children,
    onSelect,
    disabled,
  }: {
    children: React.ReactNode;
    onSelect?: () => void;
    disabled?: boolean;
  }) => (
    <button type="button" disabled={disabled} onClick={() => onSelect?.()}>
      {children}
    </button>
  ),
  DropdownMenuSeparator: () => null,
}));

import { SortableYoutubeCategoryRow } from "../SortableYoutubeCategoryRow";

function RowHarness({
  onSaveMaxVideosPerDay = vi.fn().mockResolvedValue(true),
  onRequestDelete = vi.fn(),
}: {
  onSaveMaxVideosPerDay?: (max: number) => Promise<boolean>;
  onRequestDelete?: () => void;
} = {}) {
  const form = useForm<YoutubeSettingsFormSchema>({
    resolver: zodResolver(youtubeSettingsFormSchema),
    defaultValues: youtubeSettingsDtoToForm(youtubeSettings()),
  });
  const category = youtubeCategory("cat-1", { name: "Fitness", maxVideosPerDay: 5 });

  return (
    <SortableYoutubeCategoryRow
      form={form}
      categoryLimitIndex={0}
      category={category}
      row={{ name: "Fitness", displayOrder: 0 }}
      index={0}
      watched={2}
      isSavingOrder={false}
      isPersistingCategoryLimits={false}
      onNameBlur={vi.fn()}
      onSaveMaxVideosPerDay={onSaveMaxVideosPerDay}
      onNameChange={vi.fn()}
      onRequestDelete={onRequestDelete}
    />
  );
}

describe("SortableYoutubeCategoryRow", () => {
  it("renders category row controls", () => {
    setupYoutubeBrowserMocks();

    render(<RowHarness />);

    expect(screen.getByDisplayValue("Fitness")).toBeInTheDocument();
    expect(screen.getByText(/Watched today: 2/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Category actions/i }),
    ).toBeInTheDocument();
  });

  it("saves updated max videos per day from the settings dialog", async () => {
    setupYoutubeBrowserMocks();
    const onSaveMaxVideosPerDay = vi.fn().mockResolvedValue(true);

    render(<RowHarness onSaveMaxVideosPerDay={onSaveMaxVideosPerDay} />);

    fireEvent.click(screen.getByRole("button", { name: /Edit settings/i }));

    await waitFor(() => {
      expect(screen.getByRole("dialog")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Max / day"), {
      target: { value: "10" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Save settings/i }));

    await waitFor(() => {
      expect(onSaveMaxVideosPerDay).toHaveBeenCalledWith(10);
    });
  });

  it("shows validation error for invalid max videos per day", async () => {
    setupYoutubeBrowserMocks();

    render(<RowHarness />);

    fireEvent.click(screen.getByRole("button", { name: /Edit settings/i }));

    await waitFor(() => {
      expect(screen.getByRole("dialog")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Max / day"), {
      target: { value: "-1" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Save settings/i }));

    expect(screen.getByText("Must be 0 or greater")).toBeInTheDocument();
  });

  it("closes the settings dialog without saving when the value is unchanged", async () => {
    setupYoutubeBrowserMocks();

    render(<RowHarness />);

    fireEvent.click(screen.getByRole("button", { name: /Edit settings/i }));

    await waitFor(() => {
      expect(screen.getByRole("dialog")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByRole("button", { name: /Save settings/i }));

    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
  });

  it("requests delete from the row actions menu", async () => {
    setupYoutubeBrowserMocks();
    const onRequestDelete = vi.fn();

    render(<RowHarness onRequestDelete={onRequestDelete} />);

    fireEvent.click(screen.getByRole("button", { name: /Delete/i }));

    await waitFor(() => {
      expect(onRequestDelete).toHaveBeenCalled();
    });
  });
});
