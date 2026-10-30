import { useEffect, useMemo, useState } from "react";
import { Link, useLocation, useNavigate } from "@tanstack/react-router";
import { ChevronDown, ListMusic, LucideProps, Mic, Plus, Search, Settings, Star } from "lucide-react";

import { useCustomQuery } from "@/hooks/useCustomQuery";
import { cn } from "@/lib/utils";

import CreatePlaylistDialog from "@/features/guitar-tabs/components/CreatePlaylistDialog";
import { guitarTabsQueryOptions } from "@/features/guitar-tabs/queries/guitarTabsQueries";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "../ui/collapsible";
import {
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubItem,
  useSidebar,
} from "../ui/sidebar";
import { Skeleton } from "../ui/skeleton";

type GuitarTabsIcon = React.ForwardRefExoticComponent<
  Omit<LucideProps, "ref"> & React.RefAttributes<SVGSVGElement>
>;

const guitarTabPages = new Set([
  "/guitar-tabs",
  "/guitar-tabs/tuner",
  "/guitar-tabs/search",
  "/guitar-tabs/favorites",
  "/guitar-tabs/settings",
  "/guitar-tabs/tab",
]);

function isGuitarTabsPath(pathname: string) {
  return pathname === "/guitar-tabs" || pathname.startsWith("/guitar-tabs/");
}

function isPlaylistPath(pathname: string) {
  return isGuitarTabsPath(pathname) && !guitarTabPages.has(pathname);
}

function AppSidebarGuitarTabsGroup({
  title,
  icon: Icon,
}: {
  title: string;
  icon: GuitarTabsIcon;
}) {
  const { open, isHovered, openMobile, isMobile, setOpenMobile } = useSidebar();
  const navigate = useNavigate();
  const [createOpen, setCreateOpen] = useState(false);

  const pathname = useLocation({
    select: (location) => location.pathname,
  });

  const isSectionActive = useMemo(
    () => isGuitarTabsPath(pathname),
    [pathname],
  );

  const isPlaylistActive = useMemo(
    () => isPlaylistPath(pathname),
    [pathname],
  );

  const [collapsibleOpen, setCollapsibleOpen] = useState(isSectionActive);
  const [playlistsOpen, setPlaylistsOpen] = useState(isPlaylistActive);

  useEffect(() => {
    if (isSectionActive) {
      setCollapsibleOpen(true);
    }
  }, [isSectionActive]);

  useEffect(() => {
    if (isPlaylistActive) {
      setPlaylistsOpen(true);
    }
  }, [isPlaylistActive]);

  const { query, pendingState } = useCustomQuery(
    guitarTabsQueryOptions.playlists(),
  );
  const playlists = query.data ?? [];

  const closeMobileSidebar = () => {
    if (isMobile) {
      setOpenMobile(false);
    }
  };

  return (
    <>
      <Collapsible
        open={collapsibleOpen && (open || isHovered || openMobile)}
        onOpenChange={(state) => setCollapsibleOpen(state)}
        className="group/collapsible"
      >
        <SidebarMenuItem>
          <CollapsibleTrigger asChild>
            <SidebarMenuButton>
              <Icon />
              <span
                className={cn(
                  "whitespace-nowrap",
                  isSectionActive && "font-extrabold",
                )}
              >
                {title}
              </span>
              <ChevronDown
                className={cn(
                  "ml-auto size-4 transition-all group-data-[collapsible=icon]:hidden",
                  collapsibleOpen && (open || isHovered) ? "rotate-180" : "",
                )}
              />
            </SidebarMenuButton>
          </CollapsibleTrigger>

          <CollapsibleContent>
            <SidebarMenuSub>
              <SidebarMenuSubItem>
                <SidebarMenuButton asChild>
                  <Link
                    to="/guitar-tabs/tuner"
                    activeOptions={{ exact: true, includeSearch: false }}
                    onClick={closeMobileSidebar}
                    preload="intent"
                  >
                    {({ isActive }) => (
                      <>
                        <Mic />
                        <span className={isActive ? "font-extrabold" : ""}>
                          Tuner
                        </span>
                      </>
                    )}
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuSubItem>

              <SidebarMenuSubItem>
                <SidebarMenuButton asChild>
                  <Link
                    to="/guitar-tabs/search"
                    activeOptions={{ exact: true, includeSearch: false }}
                    onClick={closeMobileSidebar}
                    preload="intent"
                  >
                    {({ isActive }) => (
                      <>
                        <Search />
                        <span className={isActive ? "font-extrabold" : ""}>
                          Search
                        </span>
                      </>
                    )}
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuSubItem>

              <SidebarMenuSubItem>
                <SidebarMenuButton asChild>
                  <Link
                    to="/guitar-tabs/favorites"
                    activeOptions={{ exact: true, includeSearch: false }}
                    onClick={closeMobileSidebar}
                    preload="intent"
                  >
                    {({ isActive }) => (
                      <>
                        <Star />
                        <span className={isActive ? "font-extrabold" : ""}>
                          Favorites
                        </span>
                      </>
                    )}
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuSubItem>

              <SidebarMenuSubItem>
                <Collapsible
                  open={playlistsOpen && (open || isHovered || openMobile)}
                  onOpenChange={(state) => setPlaylistsOpen(state)}
                  className="group/playlists"
                >
                  <CollapsibleTrigger asChild>
                    <SidebarMenuButton>
                      <ListMusic />
                      <span className={isPlaylistActive ? "font-extrabold" : ""}>
                        Playlists
                      </span>
                      <ChevronDown
                        className={cn(
                          "ml-auto size-4 transition-all group-data-[collapsible=icon]:hidden",
                          playlistsOpen && (open || isHovered) ? "rotate-180" : "",
                        )}
                      />
                    </SidebarMenuButton>
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {pendingState.isPending ? (
                        <Skeleton className="h-7 w-1/2" />
                      ) : (
                        playlists.map((playlist) => (
                          <SidebarMenuSubItem key={playlist.id}>
                            <SidebarMenuButton asChild>
                              <Link
                                to="/guitar-tabs/$playlistId"
                                params={{ playlistId: playlist.id }}
                                activeOptions={{ exact: true, includeSearch: false }}
                                onClick={closeMobileSidebar}
                                preload="intent"
                              >
                                {({ isActive }) => (
                                  <>
                                    <ListMusic />
                                    <span className={isActive ? "font-extrabold" : ""}>
                                      {playlist.name}
                                    </span>
                                  </>
                                )}
                              </Link>
                            </SidebarMenuButton>
                          </SidebarMenuSubItem>
                        ))
                      )}

                      <SidebarMenuSubItem>
                        <SidebarMenuButton
                          onClick={() => {
                            closeMobileSidebar();
                            setCreateOpen(true);
                          }}
                        >
                          <Plus />
                          <span>Create</span>
                        </SidebarMenuButton>
                      </SidebarMenuSubItem>
                    </SidebarMenuSub>
                  </CollapsibleContent>
                </Collapsible>
              </SidebarMenuSubItem>

              <SidebarMenuSubItem>
                <SidebarMenuButton asChild>
                  <Link
                    to="/guitar-tabs/settings"
                    activeOptions={{ exact: true, includeSearch: false }}
                    onClick={closeMobileSidebar}
                    preload="intent"
                  >
                    {({ isActive }) => (
                      <>
                        <Settings />
                        <span className={isActive ? "font-extrabold" : ""}>
                          Settings
                        </span>
                      </>
                    )}
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuSubItem>
            </SidebarMenuSub>
          </CollapsibleContent>
        </SidebarMenuItem>
      </Collapsible>
      <CreatePlaylistDialog
        open={createOpen}
        onOpenChange={setCreateOpen}
        onCreated={(playlistId) => {
          void navigate({
            to: "/guitar-tabs/$playlistId",
            params: { playlistId },
          });
        }}
      />
    </>
  );
}

export default AppSidebarGuitarTabsGroup;
