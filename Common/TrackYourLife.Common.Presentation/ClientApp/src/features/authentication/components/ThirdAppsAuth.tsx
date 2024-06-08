import { GitHub, Google } from "@mui/icons-material";

import { Button } from "~/chadcn/ui/button";
import { Separator } from "~/chadcn/ui/separator";
import { TAuthModes, authModesEnum } from "../data/enums";

type ThirdAppsAuthProps = {
  disabled: boolean;
  authMode: TAuthModes;
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
          OR {authMode === authModesEnum.logIn ? "LOG IN" : "SIGN UP"} WITH
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
          <GitHub />
          <span>Github</span>
        </Button>
        <Button
          type="button"
          variant="outline"
          className="flex-grow space-x-2"
          disabled={disabled}
        >
          <Google />
          <p>Google</p>
        </Button>
      </div>
    </div>
  );
};

export default ThirdAppsAuth;
