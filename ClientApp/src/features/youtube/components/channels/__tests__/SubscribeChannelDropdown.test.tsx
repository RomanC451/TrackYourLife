import { fireEvent, render, screen } from "@testing-library/react";

import { beforeEach, describe, expect, it, vi } from "vitest";



import { youtubeChannel, youtubeSettings } from "@/features/youtube/__tests__/fixtures";

import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";



const { mockUseQuery, mockAddMutate, mockRemoveMutate } = vi.hoisted(() => ({

  mockUseQuery: vi.fn(),

  mockAddMutate: vi.fn(),

  mockRemoveMutate: vi.fn(),

}));



vi.mock("@tanstack/react-query", async (importOriginal) => {

  const actual = await importOriginal<typeof import("@tanstack/react-query")>();

  return {

    ...actual,

    useQuery: (...args: unknown[]) => mockUseQuery(...args),

  };

});

vi.mock("../../../mutations/useAddChannelMutation", () => ({

  default: () => ({ mutate: mockAddMutate, isPending: false }),

}));

vi.mock("../../../mutations/useRemoveChannelMutation", () => ({

  default: () => ({

    mutate: mockRemoveMutate,

    isPending: false,

    isDelayedPending: false,

  }),

}));

vi.mock("@tanstack/react-router", () => ({

  Link: ({ children }: { children: React.ReactNode }) => <a href="/settings">{children}</a>,

}));

vi.mock("@/components/ui/dropdown-menu", () => ({

  DropdownMenu: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,

  DropdownMenuTrigger: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,

  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,

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

  DropdownMenuLabel: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,

  DropdownMenuSeparator: () => null,

}));



import SubscribeChannelDropdown from "../SubscribeChannelDropdown";



function mockQueries({

  settings = youtubeSettings(),

  channels = [] as ReturnType<typeof youtubeChannel>[],

  settingsPending = false,

  channelsPending = false,

} = {}) {

  let callCount = 0;

  mockUseQuery.mockImplementation(() => {

    callCount += 1;

    if (callCount % 2 === 1) {

      return { data: settings, isPending: settingsPending };

    }

    return { data: channels, isPending: channelsPending };

  });

}



describe("SubscribeChannelDropdown", () => {

  beforeEach(() => {

    setupYoutubeBrowserMocks();

    vi.clearAllMocks();

  });



  it("renders subscribe trigger for unsubscribed channels", () => {

    mockQueries();



    render(

      <SubscribeChannelDropdown

        channelId="ch-1"

        channelName="Fitness Channel"

        showLabel

      />,

    );



    expect(screen.getByRole("button", { name: /Subscribe/i })).toBeInTheDocument();

    expect(screen.getByText("Subscribe")).toBeInTheDocument();

  });



  it("subscribes to a selected category", () => {

    mockQueries();



    render(

      <SubscribeChannelDropdown channelId="ch-1" channelName="Fitness Channel" />,

    );



    fireEvent.click(screen.getByRole("button", { name: /^Fitness$/i }));



    expect(mockAddMutate).toHaveBeenCalledWith({

      youtubeChannelId: "ch-1",

      youtubeCategoryId: "11111111-1111-4111-8111-111111111111",

      channelName: "Fitness Channel",

      categoryName: "Fitness",

    });

  });



  it("unsubscribes from a subscribed channel", () => {

    mockQueries({ channels: [youtubeChannel("ch-1")] });



    render(

      <SubscribeChannelDropdown

        channelId="ch-1"

        channelName="Fitness Channel"

        showLabel

      />,

    );



    fireEvent.click(

      screen.getByRole("button", { name: /Unsubscribe from Fitness Channel/i }),

    );



    expect(mockRemoveMutate).toHaveBeenCalledWith({ youtubeChannelId: "ch-1" });

    expect(screen.getByText("Unsubscribe")).toBeInTheDocument();

  });



  it("shows loading state while queries are pending", () => {

    mockQueries({ settingsPending: true, channelsPending: true });



    render(

      <SubscribeChannelDropdown channelId="ch-1" channelName="Fitness Channel" />,

    );



    expect(screen.getByRole("button", { name: /Subscribe/i })).not.toBeDisabled();

    expect(document.querySelector(".animate-spin")).toBeInTheDocument();

  });



  it("prompts to add a category when none exist", () => {

    mockQueries({ settings: youtubeSettings({ categories: [] }) });



    render(

      <SubscribeChannelDropdown channelId="ch-1" channelName="Fitness Channel" />,

    );



    expect(screen.getByRole("button", { name: /Subscribe/i })).toBeDisabled();

    expect(screen.getByText(/Add a category first/i)).toBeInTheDocument();

    expect(screen.getByRole("link", { name: /Settings/i })).toBeInTheDocument();

  });

});


