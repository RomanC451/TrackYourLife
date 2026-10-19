import { render, screen } from "@testing-library/react";
import { Guitar } from "lucide-react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { playlists } = vi.hoisted(() => ({
  playlists: [
    {
      id: "acoustic",
      name: "Acoustic",
      createdOnUtc: "2026-10-19T00:00:00Z",
      modifiedOnUtc: null,
    },
    {
      id: "blues",
      name: "Blues",
      createdOnUtc: "2026-10-19T01:00:00Z",
      modifiedOnUtc: null,
    },
  ],
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({
    screenSize: { width: 1400 },
    queryToolsRef: { current: null },
    routerToolsRef: { current: null },
  }),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: () => ({
    query: {
      data: playlists,
      isPending: false,
      isError: false,
    },
    pendingState: { isPending: false },
  }),
}));

vi.mock("@/components/ui/sidebar", () => ({
  SidebarMenuButton: ({
    children,
    onClick,
    asChild,
  }: {
    children: React.ReactNode;
    onClick?: () => void;
    asChild?: boolean;
  }) => (asChild ? children : <button onClick={onClick}>{children}</button>),
  SidebarMenuItem: ({ children }: { children: React.ReactNode }) => (
    <li>{children}</li>
  ),
  SidebarMenuSub: ({ children }: { children: React.ReactNode }) => (
    <ul>{children}</ul>
  ),
  SidebarMenuSubItem: ({ children }: { children: React.ReactNode }) => (
    <li>{children}</li>
  ),
  useSidebar: () => ({
    open: true,
    isHovered: false,
    openMobile: false,
    isMobile: false,
    state: "expanded",
    setOpen: vi.fn(),
    setOpenMobile: vi.fn(),
    setIsHovered: vi.fn(),
    toggleSidebar: vi.fn(),
  }),
}));

vi.mock("@/components/ui/collapsible", () => ({
  Collapsible: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  CollapsibleTrigger: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
  CollapsibleContent: ({ children }: { children: React.ReactNode }) => (
    <div>{children}</div>
  ),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({
    children,
    to,
  }: {
    children:
      | React.ReactNode
      | ((state: { isActive: boolean }) => React.ReactNode);
    to: string;
  }) => {
    const content =
      typeof children === "function" ? children({ isActive: false }) : children;
    return <a href={to}>{content}</a>;
  },
  useLocation: ({
    select,
  }: {
    select: (location: { pathname: string }) => string;
  }) => select({ pathname: "/guitar-tabs/favorites" }),
  useNavigate: () => vi.fn(),
}));

import AppSidebarGuitarTabsGroup from "../AppSidebarGuitarTabsGroup";

describe("AppSidebarGuitarTabsGroup", () => {
  beforeEach(() => {
    playlists.splice(
      0,
      playlists.length,
      {
        id: "acoustic",
        name: "Acoustic",
        createdOnUtc: "2026-10-19T00:00:00Z",
        modifiedOnUtc: null,
      },
      {
        id: "blues",
        name: "Blues",
        createdOnUtc: "2026-10-19T01:00:00Z",
        modifiedOnUtc: null,
      },
    );
  });

  it("lists Search, Favorites, Settings, then playlists, then Create playlist", () => {
    render(<AppSidebarGuitarTabsGroup title="Guitar Tabs" icon={Guitar} />, {
      wrapper: createQueryClientWrapper(),
    });

    const search = screen.getByRole("link", { name: "Search" });
    const favorites = screen.getByRole("link", { name: "Favorites" });
    const settings = screen.getByRole("link", { name: "Settings" });
    const acoustic = screen.getByRole("link", { name: "Acoustic" });
    const blues = screen.getByRole("link", { name: "Blues" });
    const createPlaylist = screen.getByRole("button", { name: "Create playlist" });

    expect(search).toHaveAttribute("href", "/guitar-tabs/search");
    expect(settings).toHaveAttribute("href", "/guitar-tabs/settings");
    expect(
      search.compareDocumentPosition(favorites) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(
      favorites.compareDocumentPosition(settings) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(
      settings.compareDocumentPosition(acoustic) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(
      acoustic.compareDocumentPosition(blues) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(
      blues.compareDocumentPosition(createPlaylist) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(screen.queryByRole("link", { name: "Playlists" })).not.toBeInTheDocument();
  });
});
