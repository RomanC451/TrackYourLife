import { useQuery } from "@tanstack/react-query";
import type { ReactNode } from "react";

import ActiveReadingSessionFloatingDock from "../components/ActiveReadingSessionFloatingDock";
import { readingSessionsQueryOptions } from "../../queries/readingQueries";
import { ReadingTimerProvider } from "./ReadingTimerContext";

function ReadingSessionHostProvider({ children }: { children: ReactNode }) {
  const { data: session } = useQuery(readingSessionsQueryOptions.active);

  return (
    <ReadingTimerProvider sessionId={session?.id ?? "inactive"}>
      {children}
      {session ? <ActiveReadingSessionFloatingDock session={session} /> : null}
    </ReadingTimerProvider>
  );
}

export default ReadingSessionHostProvider;
