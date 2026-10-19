import {
  getFavoriteGuitarTabs,
  getGuitarPlaylist,
  getGuitarPlaylistSongs,
  getGuitarPlaylists,
  getGuitarTab,
  getSavedGuitarTab,
  searchGuitarTabs,
} from "../api/guitarTabsApi";

export const guitarTabsQueryKeys = {
  all: ["guitar-tabs"] as const,
  playlists: () => [...guitarTabsQueryKeys.all, "playlists"] as const,
  playlist: (playlistId: string) =>
    [...guitarTabsQueryKeys.all, "playlist", playlistId] as const,
  tabSearch: (value: string, page: number) =>
    [...guitarTabsQueryKeys.all, "search", value, page] as const,
  tab: (url: string) => [...guitarTabsQueryKeys.all, "tab", url] as const,
  savedTabs: () => [...guitarTabsQueryKeys.all, "saved-tab"] as const,
  savedTab: (url: string) =>
    [...guitarTabsQueryKeys.savedTabs(), url] as const,
  favorites: () => [...guitarTabsQueryKeys.all, "favorites"] as const,
  playlistSongsAll: () =>
    [...guitarTabsQueryKeys.all, "playlist-songs"] as const,
  playlistSongs: (playlistId: string) =>
    [...guitarTabsQueryKeys.playlistSongsAll(), playlistId] as const,
};

export const guitarTabsQueryOptions = {
  playlists: () =>
    ({
      queryKey: guitarTabsQueryKeys.playlists(),
      queryFn: getGuitarPlaylists,
    }) as const,
  playlist: (playlistId: string) =>
    ({
      queryKey: guitarTabsQueryKeys.playlist(playlistId),
      queryFn: () => getGuitarPlaylist(playlistId),
    }) as const,
  tabSearch: (value: string, page: number) =>
    ({
      queryKey: guitarTabsQueryKeys.tabSearch(value, page),
      queryFn: () => searchGuitarTabs(value, page),
    }) as const,
  tab: (url: string) =>
    ({
      queryKey: guitarTabsQueryKeys.tab(url),
      queryFn: () => getGuitarTab(url),
    }) as const,
  savedTab: (url: string) =>
    ({
      queryKey: guitarTabsQueryKeys.savedTab(url),
      queryFn: () => getSavedGuitarTab(url),
    }) as const,
  favorites: () =>
    ({
      queryKey: guitarTabsQueryKeys.favorites(),
      queryFn: getFavoriteGuitarTabs,
    }) as const,
  playlistSongs: (playlistId: string) =>
    ({
      queryKey: guitarTabsQueryKeys.playlistSongs(playlistId),
      queryFn: () => getGuitarPlaylistSongs(playlistId),
    }) as const,
};
