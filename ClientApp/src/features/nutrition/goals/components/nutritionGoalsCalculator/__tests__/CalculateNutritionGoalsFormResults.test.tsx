import { act, fireEvent, render, screen } from "@testing-library/react";

import { beforeEach, describe, expect, it, vi } from "vitest";



import { macroGoals } from "@/features/nutrition/__tests__/fixtures";

import { queryClient } from "@/queryClient";



import CalculateNutritionGoalsFormResults from "../CalculateNutritionGoalsFormResults";



const mockMutate = vi.hoisted(() => vi.fn());

const mockSuspenseQuery = vi.hoisted(() =>

  vi.fn(() => ({ data: macroGoals() })),

);

const mockPendingState = vi.hoisted(() => ({

  isPending: false,

  isDelayedPending: false,

}));



vi.mock("@tanstack/react-query", async (importOriginal) => {

  const actual = await importOriginal<typeof import("@tanstack/react-query")>();

  return {

    ...actual,

    useSuspenseQuery: () => mockSuspenseQuery(),

  };

});



vi.mock(

  "@/features/nutrition/goals/mutations/useUpdateNutritionGoalsMutation",

  () => ({

    default: () => ({

      mutate: mockMutate,

      pendingState: mockPendingState,

    }),

  }),

);



describe("CalculateNutritionGoalsFormResults", () => {

  beforeEach(() => {

    vi.clearAllMocks();

    mockSuspenseQuery.mockReturnValue({ data: macroGoals() });

    mockPendingState.isPending = false;

    mockPendingState.isDelayedPending = false;

    mockMutate.mockImplementation(

      (_vars, options?: { onSuccess?: () => void }) => {

        options?.onSuccess?.();

      },

    );

  });



  it("renders calculated goal values", () => {

    render(<CalculateNutritionGoalsFormResults />);



    expect(screen.getByDisplayValue("2000")).toBeInTheDocument();

    expect(screen.getByDisplayValue("150")).toBeInTheDocument();

  });



  it("renders nothing when goals are unavailable", () => {

    mockSuspenseQuery.mockReturnValue({ data: undefined as never });



    const { container } = render(<CalculateNutritionGoalsFormResults />);



    expect(container).toBeEmptyDOMElement();

  });



  it("toggles edit mode for goal fields", () => {

    render(<CalculateNutritionGoalsFormResults />);



    fireEvent.click(screen.getByRole("button", { name: "Edit" }));

    expect(screen.getByRole("button", { name: "Save Changes" })).toBeInTheDocument();

    expect(screen.getByRole("button", { name: "Cancel" })).toBeInTheDocument();



    fireEvent.click(screen.getByRole("button", { name: "Cancel" }));

    expect(screen.getByRole("button", { name: "Edit" })).toBeInTheDocument();

  });



  it("saves edited goals and invalidates overview queries", async () => {

    const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries");



    render(<CalculateNutritionGoalsFormResults />);



    await act(async () => {

      fireEvent.click(screen.getByRole("button", { name: "Edit" }));

    });



    await act(async () => {

      fireEvent.click(screen.getByRole("button", { name: "Save Changes" }));

    });



    expect(mockMutate).toHaveBeenCalledWith(

      expect.objectContaining({

        calories: 2000,

        protein: 150,

        carbohydrates: 220,

        fats: 70,

        force: true,

      }),

      expect.objectContaining({ onSuccess: expect.any(Function) }),

    );

    expect(invalidateSpy).toHaveBeenCalledWith({

      queryKey: ["dailyNutritionOverviews"],

    });

    expect(screen.getByRole("button", { name: "Edit" })).toBeInTheDocument();

  });



  it("shows validation errors when saving invalid goal values", async () => {

    render(<CalculateNutritionGoalsFormResults />);



    await act(async () => {

      fireEvent.click(screen.getByRole("button", { name: "Edit" }));

    });



    fireEvent.change(screen.getByDisplayValue("2000"), {

      target: { value: "0" },

    });



    await act(async () => {

      fireEvent.click(screen.getByRole("button", { name: "Save Changes" }));

    });



    expect(

      await screen.findByText("Calories must be greater than 0"),

    ).toBeInTheDocument();

    expect(mockMutate).not.toHaveBeenCalled();

  });



  it("disables save and cancel while the update mutation is pending", async () => {

    mockPendingState.isPending = true;



    render(<CalculateNutritionGoalsFormResults />);



    await act(async () => {

      fireEvent.click(screen.getByRole("button", { name: "Edit" }));

    });



    expect(screen.getByRole("button", { name: "Save Changes" })).toBeDisabled();

    expect(screen.getByRole("button", { name: "Cancel" })).toBeDisabled();

  });

});


