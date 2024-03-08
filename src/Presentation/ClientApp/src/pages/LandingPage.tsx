import { Link } from "@tanstack/react-router";
import React from "react";
import { Button } from "~/chadcn/ui/button";
import FullSizeCenteredLayout from "~/layouts/FullSizeCenteredLayout";
import RootLayout from "~/layouts/RootLayout";

const LandingPage: React.FC = (): JSX.Element => {
  return (
    <RootLayout>
      <FullSizeCenteredLayout>
        <Link to="/auth" preload={false}>
          <Button type="button">Authentication page</Button>
        </Link>
      </FullSizeCenteredLayout>
    </RootLayout>
  );
};

export default LandingPage;
