import React, { useEffect } from "react";
import { useNavigate } from "@tanstack/react-router";

const EmailVerificationPage: React.FC = (): React.JSX.Element => {
  const navigate = useNavigate();

  useEffect(() => {
    setTimeout(() => {
      navigate({ to: "/auth" });
    }, 3000);
  }, [navigate]);

  return (
    <div className="grid min-h-[100vh] w-full place-content-center">
      <div className="flex items-center gap-4">Success</div>
    </div>
  );
};

export default EmailVerificationPage;
