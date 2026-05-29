import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import PasswordInput from "@/components/ui/password-input";

import {
  setYoutubeSettingsPasswordFormSchema,
  SetYoutubeSettingsPasswordFormSchema,
} from "../../data/youtubeSettingsSchemas";
import { useSetYoutubeSettingsPasswordMutation } from "../../mutations/useYoutubeSettingsPasswordMutations";
import { YOUTUBE_SETTINGS_PASSWORD_HINT } from "../../utils/generateYoutubeSettingsPassword";

import ChangeYoutubeSettingsPasswordDialog from "./ChangeYoutubeSettingsPasswordDialog";
import GeneratePasswordModal from "./GeneratePasswordModal";
import RemoveYoutubeSettingsLockDialog from "./RemoveYoutubeSettingsLockDialog";

interface YoutubeSettingsLockSectionProps {
  hasSettingsPassword: boolean;
}

function YoutubeSettingsLockSection({
  hasSettingsPassword,
}: YoutubeSettingsLockSectionProps) {
  const [changeDialogOpen, setChangeDialogOpen] = useState(false);
  const [removeDialogOpen, setRemoveDialogOpen] = useState(false);
  const [generateOpen, setGenerateOpen] = useState(false);

  const setPasswordMutation = useSetYoutubeSettingsPasswordMutation();

  const setForm = useForm<SetYoutubeSettingsPasswordFormSchema>({
    resolver: zodResolver(setYoutubeSettingsPasswordFormSchema),
    defaultValues: { newPassword: "", confirmPassword: "" },
  });

  const description = hasSettingsPassword
    ? "Change or remove the password required to edit settings on this page."
    : "Optional. When set, you must enter this password each time you open this page to change settings.";

  return (
    <>
      <section className="space-y-4 rounded-lg border p-6">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold">Settings lock</h2>
          <p className="text-sm text-muted-foreground">{description}</p>
        </div>

        {hasSettingsPassword ? (
          <div className="flex flex-wrap gap-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => setChangeDialogOpen(true)}
            >
              Change password
            </Button>
            <Button
              type="button"
              variant="destructive"
              onClick={() => setRemoveDialogOpen(true)}
            >
              Remove lock
            </Button>
          </div>
        ) : (
          <Form {...setForm}>
            <form
              className="space-y-4"
              onSubmit={setForm.handleSubmit(async (data) => {
                await setPasswordMutation.mutateAsync({
                  newPassword: data.newPassword,
                });
                setForm.reset();
              })}
            >
              <FormField
                control={setForm.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>New password</FormLabel>
                    <FormDescription>{YOUTUBE_SETTINGS_PASSWORD_HINT}</FormDescription>
                    <FormControl>
                      <PasswordInput autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={setForm.control}
                name="confirmPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirm password</FormLabel>
                    <FormControl>
                      <PasswordInput autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <div className="flex flex-wrap gap-2">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setGenerateOpen(true)}
                >
                  Generate password
                </Button>
                <ButtonWithLoading
                  type="submit"
                  isLoading={setPasswordMutation.isPending}
                  disabled={setPasswordMutation.isPending}
                >
                  Enable settings lock
                </ButtonWithLoading>
              </div>
            </form>
          </Form>
        )}
      </section>

      <GeneratePasswordModal
        open={generateOpen}
        onOpenChange={setGenerateOpen}
        onUsePassword={(password) => {
          setForm.setValue("newPassword", password, { shouldValidate: true });
          setForm.setValue("confirmPassword", password, { shouldValidate: true });
        }}
      />

      <ChangeYoutubeSettingsPasswordDialog
        open={changeDialogOpen}
        onOpenChange={setChangeDialogOpen}
      />

      <RemoveYoutubeSettingsLockDialog
        open={removeDialogOpen}
        onOpenChange={setRemoveDialogOpen}
      />
    </>
  );
}

export default YoutubeSettingsLockSection;
