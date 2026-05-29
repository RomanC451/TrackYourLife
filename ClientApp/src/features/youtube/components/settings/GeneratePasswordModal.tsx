import { useState } from "react";
import { Copy, Check } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import {
  generateYoutubeSettingsPassword,
  YOUTUBE_SETTINGS_PASSWORD_HINT,
} from "../../utils/generateYoutubeSettingsPassword";

interface GeneratePasswordModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onUsePassword: (password: string) => void;
}

function GeneratePasswordModal({
  open,
  onOpenChange,
  onUsePassword,
}: GeneratePasswordModalProps) {
  const [generated, setGenerated] = useState("");
  const [confirmedSaved, setConfirmedSaved] = useState(false);
  const [copied, setCopied] = useState(false);

  const handleOpenChange = (next: boolean) => {
    if (!next) {
      setGenerated("");
      setConfirmedSaved(false);
      setCopied(false);
    }
    onOpenChange(next);
  };

  const handleGenerate = () => {
    setGenerated(generateYoutubeSettingsPassword());
    setConfirmedSaved(false);
    setCopied(false);
  };

  const handleCopy = async () => {
    if (!generated) {
      return;
    }
    await navigator.clipboard.writeText(generated);
    setCopied(true);
  };

  const handleUse = () => {
    if (!generated || !confirmedSaved) {
      return;
    }
    onUsePassword(generated);
    handleOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Generate settings password</DialogTitle>
          <DialogDescription>{YOUTUBE_SETTINGS_PASSWORD_HINT}</DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="generated-password">Generated password</Label>
            <div className="flex gap-2">
              <Input
                id="generated-password"
                readOnly
                value={generated}
                placeholder="Click Generate"
              />
              <Button
                type="button"
                variant="outline"
                size="icon"
                onClick={handleCopy}
                disabled={!generated}
                aria-label="Copy password"
              >
                {copied ? (
                  <Check className="h-4 w-4" />
                ) : (
                  <Copy className="h-4 w-4" />
                )}
              </Button>
            </div>
          </div>

          <Button type="button" variant="secondary" onClick={handleGenerate}>
            Generate
          </Button>

          <label className="flex items-start gap-2 text-sm">
            <input
              type="checkbox"
              className="mt-1"
              checked={confirmedSaved}
              onChange={(e) => setConfirmedSaved(e.target.checked)}
              disabled={!generated}
            />
            <span>I have saved this password somewhere safe</span>
          </label>
        </div>

        <DialogFooter>
          <Button type="button" variant="outline" onClick={() => handleOpenChange(false)}>
            Cancel
          </Button>
          <Button
            type="button"
            onClick={handleUse}
            disabled={!generated || !confirmedSaved}
          >
            Use this password
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default GeneratePasswordModal;
