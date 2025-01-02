import { Link } from "@tanstack/react-router";
import React from "react";

import { Button } from "@/components/ui/button";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";

const LandingPage: React.FC = (): JSX.Element => {
  return (
    <FullSizeCenteredLayout className="h-screen">
      <Link to="/auth" preload={false}>
        <Button type="button">Authentication page</Button>
      </Link>
    </FullSizeCenteredLayout>
  );
};

export default LandingPage;
