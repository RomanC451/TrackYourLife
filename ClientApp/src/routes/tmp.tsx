import { createFileRoute } from "@tanstack/react-router";

import EmailVerificationPage from "@/pages/EmailVerificationPage";

export const Route = createFileRoute("/tmp")({
  component: EmailVerificationPage,
});
