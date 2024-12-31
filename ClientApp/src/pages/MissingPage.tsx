import React from "react";
import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";
import RootPageLayout from "@/layouts/pageLayouts/RootPageLayout";

const MissingPage: React.FC = (): JSX.Element => {
  return (
    <RootPageLayout>
      <FullSizeCenteredLayout className="-mt-40 flex flex-col gap-2 text-2xl">
        <p>Ooops...</p>
        <p>Page not found</p>

        <Link to="/home" preload={false}>
          <Button type="button">Go home</Button>
        </Link>
      </FullSizeCenteredLayout>
    </RootPageLayout>
  );
};

export default MissingPage;
