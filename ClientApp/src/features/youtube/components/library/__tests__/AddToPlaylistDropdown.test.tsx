import { fireEvent, render, screen, waitFor } from "@testing-library/react";

import { beforeEach, describe, expect, it, vi } from "vitest";



import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";



const { mockUseQuery, mockAddMutate } = vi.hoisted(() => ({

  mockUseQuery: vi.fn(),

  mockAddMutate: vi.fn(),

}));



vi.mock("@tanstack/react-query", async (importOriginal) => {

  const actual = await importOriginal<typeof import("@tanstack/react-query")>();

  return {

    ...actual,

    useQuery: (...args: unknown[]) => mockUseQuery(...args),

  };

});

vi.mock("../../../mutations/useLibraryPlaylistMutations", () => ({

  useAddVideoToPlaylistMutation: () => ({

    mutate: mockAddMutate,

    isPending: false,

  }),

}));

vi.mock("../CreatePlaylistDialog", () => ({

  default: ({

    open,

    onCreated,

  }: {

    open: boolean;

    onCreated: (id: string) => void;

  }) =>

    open ? (

      <button type="button" onClick={() => onCreated("pl-new")}>

        Confirm create playlist

      </button>

    ) : null,

}));

vi.mock("@/components/ui/dropdown-menu", () => ({

  DropdownMenu: ({

    children,

    open,

    onOpenChange,

  }: {

    children: React.ReactNode;

    open?: boolean;

    onOpenChange?: (open: boolean) => void;

  }) => (

    <div data-open={open} onClick={() => onOpenChange?.(true)}>

      {children}

    </div>

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

  DropdownMenuLabel: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,

  DropdownMenuSeparator: () => null,

}));



import AddToPlaylistDropdown from "../AddToPlaylistDropdown";



describe("AddToPlaylistDropdown", () => {

  beforeEach(() => {

    setupYoutubeBrowserMocks();

    vi.clearAllMocks();

    mockUseQuery.mockReturnValue({

      data: [{ id: "pl-1", name: "Watch later" }],

      isPending: false,

    });

  });



  it("renders playlist trigger when playlists are available", () => {

    render(<AddToPlaylistDropdown videoId="video-1" showLabel />);



    expect(screen.getByRole("button", { name: /Add to playlist/i })).toBeInTheDocument();

    expect(screen.getByText("Playlist")).toBeInTheDocument();

  });



  it("adds a video to an existing playlist", () => {

    render(<AddToPlaylistDropdown videoId="video-1" />);



    fireEvent.click(screen.getByRole("button", { name: /Watch later/i }));



    expect(mockAddMutate).toHaveBeenCalledWith(

      { playlistId: "pl-1", videoId: "video-1" },

      expect.objectContaining({ onSuccess: expect.any(Function) }),

    );

  });



  it("shows only create playlist when the library is empty", () => {

    mockUseQuery.mockReturnValue({ data: [], isPending: false });



    render(<AddToPlaylistDropdown videoId="video-1" />);



    expect(screen.getByRole("button", { name: /Create playlist/i })).toBeInTheDocument();

    expect(screen.queryByRole("button", { name: /Watch later/i })).not.toBeInTheDocument();

  });



  it("shows loading spinner while playlists load", () => {

    mockUseQuery.mockReturnValue({ data: undefined, isPending: true });



    render(<AddToPlaylistDropdown videoId="video-1" />);



    expect(document.querySelector(".animate-spin")).toBeInTheDocument();

  });



  it("creates a playlist and adds the video", async () => {

    mockUseQuery.mockReturnValue({ data: [], isPending: false });



    render(<AddToPlaylistDropdown videoId="video-1" />);



    fireEvent.click(screen.getByRole("button", { name: /Create playlist/i }));



    await waitFor(() => {

      expect(screen.getByRole("button", { name: /Confirm create playlist/i })).toBeInTheDocument();

    });



    fireEvent.click(screen.getByRole("button", { name: /Confirm create playlist/i }));



    expect(mockAddMutate).toHaveBeenCalledWith({

      playlistId: "pl-new",

      videoId: "video-1",

    });

  });

});


