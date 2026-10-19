import axios from "axios";

import { BASE_PATH } from "@/services/openapi/base";

export type GuitarPlaylist = {
  id: string;
  name: string;
  createdOnUtc: string;
  modifiedOnUtc: string | null;
};

export type GuitarPlaylistIdResponse = {
  id: string;
};

export type GuitarTabSearchResult = {
  id: number | null;
  songName: string;
  artistName: string;
  type: string;
  votes: number;
  rating: number | null;
  tonality: string | null;
  version: number;
  part: string;
  tabUrl: string;
  artistUrl: string | null;
};

export type GuitarTabSearchResponse = {
  query: string;
  resultsCount: number;
  page: number;
  totalPages: number;
  results: GuitarTabSearchResult[];
};

export type GuitarTabChord = {
  name: string;
  frets: number[];
  position: number;
  barreFret: number | null;
  barreFromString: number | null;
  barreToString: number | null;
};

export type GuitarTabContent = {
  id: number;
  songName: string;
  artistName: string;
  type: string;
  tonality: string | null;
  tuningName: string | null;
  tuning: string | null;
  capo: number | null;
  rating: number | null;
  votes: number;
  content: string;
  tabUrl: string;
  chords: GuitarTabChord[];
  youtubeUrl: string | null;
};

export type GuitarTabLibraryState = {
  isFavorite: boolean;
  playlistIds: string[];
  youtubeUrl: string | null;
};

export type SavedGuitarTab = {
  id: string;
  songName: string;
  artistName: string;
  type: string;
  tabUrl: string;
};

export type GuitarTabSaveRequest = {
  tabUrl: string;
  songName: string;
  artistName: string;
  type: string;
};

const playlistsPath = `${BASE_PATH}/api/guitar-tabs/playlists`;
const searchPath = `${BASE_PATH}/api/guitar-tabs/search`;
const tabPath = `${BASE_PATH}/api/guitar-tabs/tabs`;
const savedTabsPath = `${BASE_PATH}/api/guitar-tabs/saved-tabs`;
const favoritesPath = `${BASE_PATH}/api/guitar-tabs/favorites`;

export async function getGuitarPlaylists(): Promise<GuitarPlaylist[]> {
  const response = await axios.get<GuitarPlaylist[]>(playlistsPath);
  return response.data;
}

export async function getGuitarPlaylist(
  playlistId: string,
): Promise<GuitarPlaylist> {
  const response = await axios.get<GuitarPlaylist>(
    `${playlistsPath}/${playlistId}`,
  );
  return response.data;
}

export async function createGuitarPlaylist(
  name: string,
): Promise<GuitarPlaylistIdResponse> {
  const response = await axios.post<GuitarPlaylistIdResponse>(playlistsPath, {
    name,
  });
  return response.data;
}

export async function updateGuitarPlaylist(
  playlistId: string,
  name: string,
): Promise<void> {
  await axios.put(`${playlistsPath}/${playlistId}`, { name });
}

export async function deleteGuitarPlaylist(playlistId: string): Promise<void> {
  await axios.delete(`${playlistsPath}/${playlistId}`);
}

export async function searchGuitarTabs(
  value: string,
  page: number,
): Promise<GuitarTabSearchResponse> {
  const response = await axios.get<GuitarTabSearchResponse>(searchPath, {
    params: { value, page },
  });
  return response.data;
}

export async function getGuitarTab(url: string): Promise<GuitarTabContent> {
  const response = await axios.get<GuitarTabContent>(tabPath, {
    params: { url },
  });
  return response.data;
}

export async function getSavedGuitarTab(
  url: string,
): Promise<GuitarTabLibraryState> {
  const response = await axios.get<GuitarTabLibraryState>(savedTabsPath, {
    params: { url },
  });
  return response.data;
}

export async function setSavedGuitarTabFavorite(
  request: GuitarTabSaveRequest & { isFavorite: boolean },
): Promise<GuitarTabLibraryState> {
  const response = await axios.put<GuitarTabLibraryState>(
    `${savedTabsPath}/favorite`,
    request,
  );
  return response.data;
}

export async function setSavedGuitarTabYoutube(
  request: GuitarTabSaveRequest & { youtubeUrl: string | null },
): Promise<GuitarTabLibraryState> {
  const response = await axios.put<GuitarTabLibraryState>(
    `${savedTabsPath}/youtube`,
    request,
  );
  return response.data;
}

export async function setSavedGuitarTabPlaylists(
  request: GuitarTabSaveRequest & { playlistIds: string[] },
): Promise<GuitarTabLibraryState> {
  const response = await axios.put<GuitarTabLibraryState>(
    `${savedTabsPath}/playlists`,
    request,
  );
  return response.data;
}

export async function getFavoriteGuitarTabs(): Promise<SavedGuitarTab[]> {
  const response = await axios.get<SavedGuitarTab[]>(favoritesPath);
  return response.data;
}

export async function getGuitarPlaylistSongs(
  playlistId: string,
): Promise<SavedGuitarTab[]> {
  const response = await axios.get<SavedGuitarTab[]>(
    `${playlistsPath}/${playlistId}/songs`,
  );
  return response.data;
}

export async function removeGuitarTabFromPlaylist(
  playlistId: string,
  songId: string,
): Promise<void> {
  await axios.delete(`${playlistsPath}/${playlistId}/songs/${songId}`);
}
