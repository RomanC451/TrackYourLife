import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

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
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PasswordInput from "@/components/ui/password-input";

import {
  removeYoutubeSettingsPasswordFormSchema,
  RemoveYoutubeSettingsPasswordFormSchema,
} from "../../data/youtubeSettingsSchemas";
import { useSetYoutubeSettingsPasswordMutation } from "../../mutations/useYoutubeSettingsPasswordMutations";

interface RemoveYoutubeSettingsLockDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function RemoveYoutubeSettingsLockDialog({
  open,
  onOpenChange,
}: RemoveYoutubeSettingsLockDialogProps) {
  const setPasswordMutation = useSetYoutubeSettingsPasswordMutation();

  const form = useForm<RemoveYoutubeSettingsPasswordFormSchema>({
    resolver: zodResolver(removeYoutubeSettingsPasswordFormSchema),
    defaultValues: { currentPassword: "" },
  });

  useEffect(() => {
    if (!open) {
      form.reset();
    }
  }, [open, form]);

  const handleOpenChange = (next: boolean) => {
    if (!next) {
      form.reset();
    }
    onOpenChange(next);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent onCloseAutoFocus={(e) => e.preventDefault()}>
        <DialogHeader>
          <DialogTitle>Remove settings lock</DialogTitle>
          <DialogDescription>
            Settings will be editable without a password on future visits.
            Enter your current password to confirm.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form
            className="space-y-4"
            onSubmit={form.handleSubmit(async (data) => {
              await setPasswordMutation.mutateAsync({
                currentPassword: data.currentPassword,
              });
              handleOpenChange(false);
            })}
          >
            <FormField
              control={form.control}
              name="currentPassword"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Current password</FormLabel>
                  <FormControl>
                    <PasswordInput autoComplete="current-password" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => handleOpenChange(false)}
              >
                Cancel
              </Button>
              <ButtonWithLoading
                type="submit"
                variant="destructive"
                isLoading={setPasswordMutation.isPending}
                disabled={setPasswordMutation.isPending}
              >
                Remove settings lock
              </ButtonWithLoading>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

export default RemoveYoutubeSettingsLockDialog;
