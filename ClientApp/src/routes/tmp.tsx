import { createFileRoute } from "@tanstack/react-router";

import { EmailVerificationSuccess } from "./email-verification";

export const Route = createFileRoute("/tmp")({
  component: EmailVerificationSuccess,
});
