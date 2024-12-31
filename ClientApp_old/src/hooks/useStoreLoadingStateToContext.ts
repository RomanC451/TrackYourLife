import { useEffect } from "react";
import { useLoadingUpdate } from "~/contexts/LoadingContext";

export default function useStoreLoadingStateToContext(
  id: string,
  loadingState: boolean,
) {
  const updateLoadingState = useLoadingUpdate();

  useEffect(() => {
    updateLoadingState(id, loadingState);
  }, [loadingState]);
}
