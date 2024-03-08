import { Link } from "@tanstack/react-router";
import React from "react";
import { Button } from "~/chadcn/ui/button";
import FullSizeCenteredLayout from "~/layouts/FullSizeCenteredLayout";
import RootLayout from "~/layouts/RootLayout";

const MissingPage: React.FC = (): JSX.Element => {
  return (
    <RootLayout>
      <FullSizeCenteredLayout className="-mt-40 flex flex-col gap-2 text-2xl">
        <p>Ooops...</p>
        <p>Page not found</p>

        <Link to="/home" preload={false}>
          <Button type="button">Go home</Button>
        </Link>
      </FullSizeCenteredLayout>
    </RootLayout>
  );
};

export default MissingPage;
