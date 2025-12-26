import React from "react";
import { useRouter } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";

const MissingPage: React.FC = (): React.JSX.Element => {
  const router = useRouter();
  const onBack = () => router.history.back();
  return (
    <FullSizeCenteredLayout className="flex h-screen flex-col gap-2 text-2xl">
      <p>Ooops...</p>
      <p>Page not found</p>

      <Button type="button" onClick={onBack}>
        Go back
      </Button>
    </FullSizeCenteredLayout>
  );
};

export default MissingPage;
