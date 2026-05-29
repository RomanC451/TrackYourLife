import { useCallback, useEffect, useState } from "react";

import { useVerifyYoutubeSettingsPasswordMutation } from "../mutations/useYoutubeSettingsPasswordMutations";

export function useYoutubeSettingsUnlock(hasSettingsPassword: boolean) {
  const [isUnlocked, setIsUnlocked] = useState(!hasSettingsPassword);
  const verifyMutation = useVerifyYoutubeSettingsPasswordMutation();

  useEffect(() => {
    setIsUnlocked(!hasSettingsPassword);
  }, [hasSettingsPassword]);

  useEffect(() => {
    return () => {
      setIsUnlocked(false);
    };
  }, []);

  const unlock = useCallback(
    async (password: string) => {
      await verifyMutation.mutateAsync({ password });
      setIsUnlocked(true);
    },
    [verifyMutation],
  );

  return {
    isUnlocked,
    unlock,
    isVerifying: verifyMutation.isPending,
    verifyError: verifyMutation.error,
    resetVerifyError: verifyMutation.reset,
  };
}
