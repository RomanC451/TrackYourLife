import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";

import { AuthMode, authModes } from "../data/enums";

type ThirdAppsAuthProps = {
  disabled: boolean;
  authMode: AuthMode;
};

const ThirdAppsAuth: React.FC<ThirdAppsAuthProps> = ({
  disabled,
  authMode,
}) => {
  return (
    <div className="space-y-3">
      <div className="flex items-center gap-2">
        <Separator className="w-auto flex-grow" />
        <span>
          OR {authMode === authModes.logIn ? "LOG IN" : "SIGN UP"} WITH
        </span>
        <Separator className="w-auto flex-grow" />
      </div>
      <div className="flex flex-wrap gap-4">
        <Button
          type="button"
          variant="outline"
          className="flex-grow space-x-2"
          disabled={disabled}
        >
          {/* // !! TODO: Add Github icon */}
          <span>Github</span>
        </Button>
        <Button
          type="button"
          variant="outline"
          className="flex-grow space-x-2"
          disabled={disabled}
        >
          {/* // !! TODO: Add Google icon */}
          <p>Google</p>
        </Button>
      </div>
    </div>
  );
};

export default ThirdAppsAuth;
