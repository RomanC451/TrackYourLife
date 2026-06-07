import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { recipe } from "@/features/nutrition/__tests__/fixtures";

const mockNavigate = vi.fn();
const mockCreateMutate = vi.fn();
const mockUpdateMutate = vi.fn();
const mockUseCustomQuery = vi.fn();
const mockSetDataIfNoSessionStorage = vi.hoisted(() => vi.fn());

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (options: { onFirstFetch?: () => void }) => {
    mockUseCustomQuery(options);
    return {
      query: { data: recipe("recipe-1", "Soup"), isLoading: false },
      isDelayedPending: false,
    };
  },
}));

const mockTrigger = vi.hoisted(() => vi.fn().mockResolvedValue(true));
const mockFormSetError = vi.hoisted(() => vi.fn());
const capturedSetErrors = vi.hoisted(() => ({
  create: null as
    | ((
        name: string,
        error: { message: string },
        options?: { shouldFocus: boolean },
      ) => void)
    | null,
  update: null as
    | ((
        name: string,
        error: { message: string },
        options?: { shouldFocus: boolean },
      ) => void)
    | null,
}));

vi.mock("@/components/forms/useCustomForm", () => ({
  default: ({
    onSubmit,
  }: {
    onSubmit: (data: {
      name: string;
      portions: number;
      weight: number;
    }) => Promise<void>;
  }) => ({
    form: {
      setError: mockFormSetError,
      setDataIfNoSessionStorage: mockSetDataIfNoSessionStorage,
      trigger: mockTrigger,
      formState: { errors: {} },
    },
    handleCustomSubmit: (event: { preventDefault: () => void }) => {
      event.preventDefault();
      void onSubmit({ name: "Soup", portions: 2, weight: 400 });
    },
  }),
}));

vi.mock("../../../mutations/useCreateRecipeMutation", () => ({
  default: ({
    onSuccess,
    setError,
  }: {
    onSuccess: (id: string) => void;
    setError: (
      name: string,
      error: { message: string },
      options?: { shouldFocus: boolean },
    ) => void;
  }) => {
    capturedSetErrors.create = setError;
    return {
      mutate: (data: unknown) => {
        mockCreateMutate(data);
        onSuccess("recipe-new");
      },
      pendingState: { isPending: false, isDelayedPending: false },
    };
  },
}));

vi.mock("../../../mutations/useUpdateRecipeMutation", () => ({
  default: ({
    onSuccess,
    setError,
  }: {
    onSuccess: () => void;
    setError: (
      name: string,
      error: { message: string },
      options?: { shouldFocus: boolean },
    ) => void;
  }) => {
    capturedSetErrors.update = setError;
    return {
      mutate: (data: unknown) => {
        mockUpdateMutate(data);
        onSuccess();
      },
      pendingState: { isPending: false, isDelayedPending: false },
    };
  },
}));

import useRecipeDialog from "../useRecipeDialog";

describe("useRecipeDialog integration", () => {
  const setTab = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    mockTrigger.mockResolvedValue(true);
  });

  it("submits create mutation and navigates to edit on success", async () => {
    const { result } = renderHook(() =>
      useRecipeDialog({
        dialogType: "create",
        setTab,
        recipeId: undefined,
      }),
    );

    act(() => {
      result.current.handleCustomSubmit({
        preventDefault: vi.fn(),
      } as unknown as React.FormEvent<HTMLFormElement>);
    });

    await waitFor(() =>
      expect(mockCreateMutate).toHaveBeenCalledWith({
        name: "Soup",
        portions: 2,
        weight: 400,
      }),
    );
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/nutrition/recipes/edit/$recipeId",
      params: { recipeId: "recipe-new" },
      search: { tab: "ingredients" },
      replace: true,
    });
  });

  it("submits update mutation and switches tab on success", async () => {
    const { result } = renderHook(() =>
      useRecipeDialog({
        dialogType: "edit",
        setTab,
        recipeId: "recipe-1",
      }),
    );

    act(() => {
      result.current.handleCustomSubmit({
        preventDefault: vi.fn(),
      } as unknown as React.FormEvent<HTMLFormElement>);
    });

    await waitFor(() =>
      expect(mockUpdateMutate).toHaveBeenCalledWith({
        name: "Soup",
        portions: 2,
        weight: 400,
      }),
    );
    expect(setTab).toHaveBeenCalledWith("ingredients");
  });

  it("does not submit when form validation fails", async () => {
    mockTrigger.mockResolvedValue(false);

    const { result } = renderHook(() =>
      useRecipeDialog({
        dialogType: "edit",
        setTab,
        recipeId: "recipe-1",
      }),
    );

    act(() => {
      result.current.handleCustomSubmit({
        preventDefault: vi.fn(),
      } as unknown as React.FormEvent<HTMLFormElement>);
    });

    await waitFor(() => expect(mockTrigger).toHaveBeenCalled());
    expect(mockUpdateMutate).not.toHaveBeenCalled();
    expect(setTab).not.toHaveBeenCalled();
  });

  it("syncs form data on first fetch", () => {
    renderHook(() =>
      useRecipeDialog({
        dialogType: "edit",
        setTab,
        recipeId: "recipe-1",
      }),
    );

    mockUseCustomQuery.mock.calls.at(-1)![0].onFirstFetch?.();

    expect(mockSetDataIfNoSessionStorage).toHaveBeenCalledWith(
      recipe("recipe-1", "Soup"),
    );
  });

  it("forwards create mutation validation errors to the form", () => {
    renderHook(() =>
      useRecipeDialog({
        dialogType: "create",
        setTab,
        recipeId: undefined,
      }),
    );

    capturedSetErrors.create!("name", { message: "Name is required" }, {
      shouldFocus: false,
    });

    expect(mockFormSetError).toHaveBeenCalledWith(
      "name",
      { message: "Name is required" },
      { shouldFocus: false },
    );
  });

  it("forwards update mutation validation errors to the form", () => {
    renderHook(() =>
      useRecipeDialog({
        dialogType: "edit",
        setTab,
        recipeId: "recipe-1",
      }),
    );

    capturedSetErrors.update!("name", { message: "Name is required" }, {
      shouldFocus: false,
    });

    expect(mockFormSetError).toHaveBeenCalledWith(
      "name",
      { message: "Name is required" },
      { shouldFocus: false },
    );
  });
});
