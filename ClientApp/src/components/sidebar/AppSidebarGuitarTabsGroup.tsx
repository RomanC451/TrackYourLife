import { useEffect, useMemo, useState } from "react";
import { Link, useLocation, useNavigate } from "@tanstack/react-router";
import { ChevronDown, ListMusic, LucideProps, Plus, Search, Star } from "lucide-react";

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

function isGuitarTabsPath(pathname: string) {
  return pathname === "/guitar-tabs" || pathname.startsWith("/guitar-tabs/");
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

  const [collapsibleOpen, setCollapsibleOpen] = useState(isSectionActive);

  useEffect(() => {
    if (isSectionActive) {
      setCollapsibleOpen(true);
    }
  }, [isSectionActive]);

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
            <SidebarMenuButton className="flex justify-between">
              <div className="inline-flex items-center gap-2">
                <Icon className="size-4" />
                <span className={isSectionActive ? "font-extrabold" : ""}>
                  {title}
                </span>
              </div>
              <ChevronDown
                className={cn(
                  "size-4 transition-all",
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
                  <span>Create playlist</span>
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
