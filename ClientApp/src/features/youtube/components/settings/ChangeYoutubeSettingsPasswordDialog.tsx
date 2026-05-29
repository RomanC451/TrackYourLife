import { useEffect, useState } from "react";
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
  changeYoutubeSettingsPasswordFormSchema,
  ChangeYoutubeSettingsPasswordFormSchema,
} from "../../data/youtubeSettingsSchemas";
import { useSetYoutubeSettingsPasswordMutation } from "../../mutations/useYoutubeSettingsPasswordMutations";

import GeneratePasswordModal from "./GeneratePasswordModal";

interface ChangeYoutubeSettingsPasswordDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function ChangeYoutubeSettingsPasswordDialog({
  open,
  onOpenChange,
}: ChangeYoutubeSettingsPasswordDialogProps) {
  const setPasswordMutation = useSetYoutubeSettingsPasswordMutation();
  const [generateOpen, setGenerateOpen] = useState(false);

  const form = useForm<ChangeYoutubeSettingsPasswordFormSchema>({
    resolver: zodResolver(changeYoutubeSettingsPasswordFormSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
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
    <>
      <Dialog open={open} onOpenChange={handleOpenChange}>
        <DialogContent onCloseAutoFocus={(e) => e.preventDefault()}>
          <DialogHeader>
            <DialogTitle>Change settings password</DialogTitle>
            <DialogDescription>
              Enter your current password and choose a new one.
            </DialogDescription>
          </DialogHeader>

          <Form {...form}>
            <form
              className="space-y-4"
              onSubmit={form.handleSubmit(async (data) => {
                await setPasswordMutation.mutateAsync({
                  currentPassword: data.currentPassword,
                  newPassword: data.newPassword,
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
              <FormField
                control={form.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>New password</FormLabel>
                    <FormControl>
                      <PasswordInput autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="confirmPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirm new password</FormLabel>
                    <FormControl>
                      <PasswordInput autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <DialogFooter className="flex-col gap-2 sm:flex-row sm:justify-between">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setGenerateOpen(true)}
                >
                  Generate password
                </Button>
                <div className="flex gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => handleOpenChange(false)}
                  >
                    Cancel
                  </Button>
                  <ButtonWithLoading
                    type="submit"
                    isLoading={setPasswordMutation.isPending}
                    disabled={setPasswordMutation.isPending}
                  >
                    Update password
                  </ButtonWithLoading>
                </div>
              </DialogFooter>
            </form>
          </Form>
        </DialogContent>
      </Dialog>

      <GeneratePasswordModal
        open={generateOpen}
        onOpenChange={setGenerateOpen}
        onUsePassword={(password) => {
          form.setValue("newPassword", password, { shouldValidate: true });
          form.setValue("confirmPassword", password, { shouldValidate: true });
        }}
      />
    </>
  );
}

export default ChangeYoutubeSettingsPasswordDialog;
