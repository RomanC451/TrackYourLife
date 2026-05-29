import { useState } from "react";
import { Lock } from "lucide-react";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import PasswordInput from "@/components/ui/password-input";
import { Label } from "@/components/ui/label";

import ResetYoutubeSettingsPasswordDialog from "./ResetYoutubeSettingsPasswordDialog";

interface YoutubeSettingsUnlockOverlayProps {
  onUnlock: (password: string) => Promise<void>;
  isVerifying: boolean;
}

function YoutubeSettingsUnlockOverlay({
  onUnlock,
  isVerifying,
}: YoutubeSettingsUnlockOverlayProps) {
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [resetDialogOpen, setResetDialogOpen] = useState(false);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    try {
      await onUnlock(password);
      setPassword("");
    } catch {
      setError("Incorrect password. Try again.");
    }
  };

  return (
    <div className="absolute inset-0 z-10 flex items-start justify-center bg-background/60 p-6 pt-16">
      <Card className="w-full max-w-md shadow-lg">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Lock className="h-5 w-5" />
            Settings locked
          </CardTitle>
          <CardDescription>
            Enter your YouTube settings password to change categories and limits.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="unlock-password">Password</Label>
              <PasswordInput
                id="unlock-password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
            {error ? (
              <p className="text-sm text-destructive">{error}</p>
            ) : null}
            <ButtonWithLoading
              type="submit"
              className="w-full"
              isLoading={isVerifying}
              disabled={!password.trim() || isVerifying}
            >
              Unlock settings
            </ButtonWithLoading>
            <Button
              type="button"
              variant="link"
              className="h-auto px-0 text-sm"
              onClick={() => setResetDialogOpen(true)}
            >
              Forgot password?
            </Button>
          </form>
        </CardContent>
      </Card>

      <ResetYoutubeSettingsPasswordDialog
        open={resetDialogOpen}
        onOpenChange={setResetDialogOpen}
      />
    </div>
  );
}

export default YoutubeSettingsUnlockOverlay;
