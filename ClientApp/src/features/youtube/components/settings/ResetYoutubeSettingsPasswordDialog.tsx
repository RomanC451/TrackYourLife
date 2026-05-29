import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { useResetYoutubeSettingsPasswordViaEmailMutation } from "../../mutations/useYoutubeSettingsPasswordMutations";

interface ResetYoutubeSettingsPasswordDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function ResetYoutubeSettingsPasswordDialog({
  open,
  onOpenChange,
}: ResetYoutubeSettingsPasswordDialogProps) {
  const resetMutation = useResetYoutubeSettingsPasswordViaEmailMutation();

  const handleOpenChange = (next: boolean) => {
    if (!next && !resetMutation.isPending) {
      onOpenChange(false);
    }
  };

  const handleConfirm = async () => {
    await resetMutation.mutateAsync();
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Email a new settings password?</DialogTitle>
          <DialogDescription>
            Your current settings password will stop working immediately. A new
            random password will be sent to your verified account email. You
            will need to enter it here to unlock settings.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => handleOpenChange(false)}
            disabled={resetMutation.isPending}
          >
            Cancel
          </Button>
          <ButtonWithLoading
            type="button"
            onClick={handleConfirm}
            isLoading={resetMutation.isPending}
            disabled={resetMutation.isPending}
          >
            Send email
          </ButtonWithLoading>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default ResetYoutubeSettingsPasswordDialog;
