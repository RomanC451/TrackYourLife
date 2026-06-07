import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import type { DragEndEvent } from "@dnd-kit/core";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeCategory, youtubeSettings } from "@/features/youtube/__tests__/fixtures";
import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";
import {
  youtubeSettingsDtoToForm,
  youtubeSettingsFormSchema,
  type YoutubeSettingsFormSchema,
} from "@/features/youtube/data/youtubeSettingsSchemas";
import type {
  DailyCategoryWatchCounterDto,
  YoutubeCategorySettingsDto,
} from "@/services/openapi";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockDeleteMutate,
  mockUpdateMutateAsync,
  mockUpdateYoutubeCategoryMetadata,
} = vi.hoisted(() => ({
  mockDeleteMutate: vi.fn(),
  mockUpdateMutateAsync: vi.fn(),
  mockUpdateYoutubeCategoryMetadata: vi.fn(),
}));

let capturedOnDragEnd: ((event: DragEndEvent) => void) | undefined;

vi.mock("../SortableYoutubeCategoryRow", () => ({
  SortableYoutubeCategoryRow: ({
    category,
    onRequestDelete,
    onNameBlur,
    onNameChange,
  }: {
    category: { id: string; name: string };
    onRequestDelete: () => void;
    onNameBlur: () => void;
    onNameChange: (name: string) => void;
  }) => (
    <div data-testid={`category-row-${category.id}`}>
      <span>{category.name}</span>
      <button type="button" onClick={onRequestDelete}>
        Delete {category.name}
      </button>
      <input
        aria-label={`Name ${category.name}`}
        defaultValue={category.name}
        onChange={(event) => onNameChange(event.target.value)}
        onBlur={onNameBlur}
      />
    </div>
  ),
}));

vi.mock("../AddYoutubeCategoryDialog", () => ({
  AddYoutubeCategoryDialog: () => <button type="button">Add category</button>,
}));

vi.mock("@dnd-kit/core", () => ({
  DndContext: ({
    children,
    onDragEnd,
  }: {
    children: React.ReactNode;
    onDragEnd?: (event: DragEndEvent) => void;
  }) => {
    capturedOnDragEnd = onDragEnd;
    return <div>{children}</div>;
  },
  closestCenter: vi.fn(),
  KeyboardSensor: vi.fn(),
  PointerSensor: vi.fn(),
  useSensor: vi.fn(),
  useSensors: vi.fn(() => []),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@dnd-kit/sortable", () => ({
  SortableContext: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  arrayMove: (items: string[], oldIndex: number, newIndex: number) => {
    const next = [...items];
    const [removed] = next.splice(oldIndex, 1);
    next.splice(newIndex, 0, removed);
    return next;
  },
  sortableKeyboardCoordinates: vi.fn(),
  verticalListSortingStrategy: {},
}));

vi.mock("../../../mutations/useYoutubeCategoryMutations", () => ({
  useDeleteYoutubeCategoryMutation: () => ({
    mutate: mockDeleteMutate,
    isPending: false,
  }),
  useUpdateYoutubeCategoryMetadataMutation: () => ({
    mutateAsync: mockUpdateMutateAsync,
    isPending: false,
  }),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockSettingsApi {
    updateYoutubeCategoryMetadata = mockUpdateYoutubeCategoryMetadata;
  }
  return { ...actual, SettingsApi: MockSettingsApi };
});

vi.mock("sonner", () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}));

import YoutubeCategoriesSection from "../YoutubeCategoriesSection";

function categoryWithChannels(
  id: "cat-1" | "cat-2",
  channelCount: number,
  overrides: Partial<YoutubeCategorySettingsDto> = {},
) {
  return youtubeCategory(id, {
    ...overrides,
    ...({
      subscribedChannelCount: channelCount,
    } as Partial<YoutubeCategorySettingsDto>),
  });
}

function SectionHarness({
  categories = [
    youtubeCategory("cat-1", { name: "Fitness", displayOrder: 0 }),
    youtubeCategory("cat-2", { name: "Learning", displayOrder: 1 }),
  ],
  onSaveCategoryLimit = vi.fn().mockResolvedValue(undefined),
}: {
  categories?: YoutubeCategorySettingsDto[];
  onSaveCategoryLimit?: (
    categoryId: string,
    maxVideosPerDay: number,
  ) => Promise<void>;
} = {}) {
  const form = useForm<YoutubeSettingsFormSchema>({
    resolver: zodResolver(youtubeSettingsFormSchema),
    defaultValues: youtubeSettingsDtoToForm(youtubeSettings({ categories })),
  });

  return (
    <YoutubeCategoriesSection
      form={form}
      categories={categories}
      dailyCounters={[
        {
          id: "counter-1",
          date: "2026-06-05",
          youtubeCategoryId: categories[0]?.id ?? "",
          videosWatchedCount: 2,
          isLoading: false,
          isDeleting: false,
        } satisfies DailyCategoryWatchCounterDto,
      ]}
      isPersistingCategoryLimits={false}
      onSaveCategoryLimit={onSaveCategoryLimit}
    />
  );
}

describe("YoutubeCategoriesSection", () => {
  beforeEach(() => {
    setupYoutubeBrowserMocks();
    vi.clearAllMocks();
    capturedOnDragEnd = undefined;
    mockUpdateYoutubeCategoryMetadata.mockResolvedValue({ data: undefined });
    mockUpdateMutateAsync.mockResolvedValue(undefined);
  });

  it("renders category rows and add action", () => {
    render(<SectionHarness />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByRole("heading", { name: "Categories" })).toBeInTheDocument();
    expect(screen.getByText("Fitness")).toBeInTheDocument();
    expect(screen.getByText("Learning")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Add category/i })).toBeInTheDocument();
  });

  it("deletes a category immediately when it has no subscribed channels", () => {
    render(<SectionHarness />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByRole("button", { name: /Delete Fitness/i }));

    expect(mockDeleteMutate).toHaveBeenCalledWith({
      id: "11111111-1111-4111-8111-111111111111",
      confirmUnsubscribeChannels: false,
    });
  });

  it("opens the delete dialog and moves channels to another category", async () => {
    render(
      <SectionHarness
        categories={[
          categoryWithChannels("cat-1", 2, {
            name: "Fitness",
            displayOrder: 0,
          }),
          categoryWithChannels("cat-2", 0, {
            name: "Learning",
            displayOrder: 1,
          }),
        ]}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Delete Fitness/i }));

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText(/Its 2 subscribed channels/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /^Delete$/i }));

    await waitFor(() => {
      expect(mockDeleteMutate).toHaveBeenCalledWith({
        id: "11111111-1111-4111-8111-111111111111",
        moveChannelsToCategoryId: "22222222-2222-4222-8222-222222222222",
      });
    });
  });

  it("deletes a category after choosing to unsubscribe its channels", async () => {
    render(
      <SectionHarness
        categories={[
          categoryWithChannels("cat-1", 1, {
            name: "Fitness",
            displayOrder: 0,
          }),
          categoryWithChannels("cat-2", 0, {
            name: "Learning",
            displayOrder: 1,
          }),
        ]}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Delete Fitness/i }));
    fireEvent.click(screen.getByLabelText(/Unsubscribe all channels/i));
    fireEvent.click(screen.getByRole("button", { name: /^Delete$/i }));

    await waitFor(() => {
      expect(mockDeleteMutate).toHaveBeenCalledWith({
        id: "11111111-1111-4111-8111-111111111111",
        confirmUnsubscribeChannels: true,
      });
    });
  });

  it("persists category order after drag and drop", async () => {
    render(<SectionHarness />, { wrapper: createQueryClientWrapper() });

    await act(async () => {
      capturedOnDragEnd?.({
        active: { id: "22222222-2222-4222-8222-222222222222" },
        over: { id: "11111111-1111-4111-8111-111111111111" },
      } as DragEndEvent);
    });

    await waitFor(() => {
      expect(mockUpdateYoutubeCategoryMetadata).toHaveBeenCalledTimes(2);
    });

    expect(mockUpdateYoutubeCategoryMetadata).toHaveBeenCalledWith(
      "22222222-2222-4222-8222-222222222222",
      { name: "Learning", displayOrder: 0 },
    );
    expect(mockUpdateYoutubeCategoryMetadata).toHaveBeenCalledWith(
      "11111111-1111-4111-8111-111111111111",
      { name: "Fitness", displayOrder: 1 },
    );
  });

  it("cancels the delete dialog without mutating", () => {
    render(
      <SectionHarness
        categories={[
          categoryWithChannels("cat-1", 1, {
            name: "Fitness",
            displayOrder: 0,
          }),
          categoryWithChannels("cat-2", 0, {
            name: "Learning",
            displayOrder: 1,
          }),
        ]}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Delete Fitness/i }));
    fireEvent.click(screen.getByRole("button", { name: /^Cancel$/i }));

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(mockDeleteMutate).not.toHaveBeenCalled();
  });

  it("saves renamed categories on blur", async () => {
    render(<SectionHarness />, { wrapper: createQueryClientWrapper() });

    const nameInput = screen.getByLabelText("Name Fitness");
    fireEvent.change(nameInput, { target: { value: "Health" } });
    await waitFor(() => {
      expect(nameInput).toHaveValue("Health");
    });
    fireEvent.blur(nameInput);

    await waitFor(() => {
      expect(mockUpdateMutateAsync).toHaveBeenCalledWith({
        id: "11111111-1111-4111-8111-111111111111",
        body: { name: "Health", displayOrder: 0 },
      });
    });
  });
});
