import React, { useEffect, useState } from "react";
import { useNavigate } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import { authModes } from "@/features/authentication/data/enums";

const EmailVerificationPage: React.FC = (): React.JSX.Element => {
  const navigate = useNavigate();

  const [count, setCount] = useState(10);

  useEffect(() => {
    const interval = setInterval(() => {
      setCount(count - 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [count]);

  useEffect(() => {
    if (count === 0) {
      navigate({ to: "/auth", search: { authMode: authModes.logIn } });
    }
  }, [count, navigate]);

  return (
    <div className="flex min-h-[50vh] w-full flex-col items-center justify-center gap-4">
      <div className="flex items-center gap-4">Email verified successfully</div>
      <div className="flex items-center gap-4">
        You will be redirected to the login page in {count} seconds.
      </div>
      <div className="flex items-center gap-4">
        <Button
          onClick={() =>
            navigate({ to: "/auth", search: { authMode: authModes.logIn } })
          }
        >
          Redirect now
        </Button>
      </div>
    </div>
  );
};

export default EmailVerificationPage;
