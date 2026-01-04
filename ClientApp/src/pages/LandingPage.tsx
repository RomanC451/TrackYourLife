import React from "react";
import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { authModes } from "@/features/authentication/data/enums";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";

const LandingPage: React.FC = (): React.JSX.Element => {
  return (
    <FullSizeCenteredLayout className="h-screen">
      <Link to="/auth" search={{ authMode: authModes.logIn }} preload={false}>
        <Button type="button">Login page</Button>
      </Link>
    </FullSizeCenteredLayout>
  );
};

export default LandingPage;
