import { useEffect, useState } from "react";
import { useSessionStorage } from "usehooks-ts";

const cyrb53 = (str: string, seed = 0) => {
  let h1 = 0xdeadbeef ^ seed,
    h2 = 0x41c6ce57 ^ seed;
  for (let i = 0, ch; i < str.length; i++) {
    ch = str.charCodeAt(i);
    h1 = Math.imul(h1 ^ ch, 2654435761);
    h2 = Math.imul(h2 ^ ch, 1597334677);
  }
  h1 =
    Math.imul(h1 ^ (h1 >>> 16), 2246822507) ^
    Math.imul(h2 ^ (h2 >>> 13), 3266489909);
  h2 =
    Math.imul(h2 ^ (h2 >>> 16), 2246822507) ^
    Math.imul(h1 ^ (h1 >>> 13), 3266489909);
  return 4294967296 * (2097151 & h2) + (h1 >>> 0);
};

function useCustomSessionStorage<T>(key?: string, queryData?: T) {
  const [isInitialized, setIsInitialized] = useState(false);

  const [sessionData, setSessionData] = useSessionStorage<T | undefined>(
    key ?? "unused",
    undefined,
  );

  const [queryDataHash, setQueryDataHash] = useSessionStorage<number>(
    `query-data-hash-${key}`,
    0,
  );

  const [constSessionData, setConstSessionData] = useState<T | undefined>(() =>
    key ? sessionData : undefined,
  );

  useEffect(() => {
    if (!isInitialized && key) {
      setIsInitialized(true);
      setConstSessionData(sessionData);
    }
  }, [isInitialized, sessionData, key]);

  useEffect(() => {
    if (queryData) {
      const newHash = cyrb53(JSON.stringify(queryData));

      if (queryDataHash !== newHash) {
        setQueryDataHash(newHash);
        setConstSessionData(queryData);
        setSessionData(queryData);
      }
    }
  }, [
    queryData,
    setSessionData,
    setConstSessionData,
    setQueryDataHash,
    queryDataHash,
  ]);

  return {
    sessionData: constSessionData,
    setSessionData,
    isDirty:
      sessionData !== undefined &&
      JSON.stringify(queryData) !== JSON.stringify(sessionData),
  } as const;
}

export default useCustomSessionStorage;
