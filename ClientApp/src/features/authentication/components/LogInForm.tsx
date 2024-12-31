import React from "react";

import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import PasswordInput from "@/components/ui/password-input";

import { authModes } from "../data/enums";
import useLogIn from "../hooks/useLogIn";
import AuthFormFooter from "./AuthFormFooter";

type LogInFormProps = {
  switchToSignUp: () => void;
  disabled: boolean;
};

const LogInForm: React.FC<LogInFormProps> = ({
  switchToSignUp,
  disabled,
}): JSX.Element => {
  const { form, onSubmit, isSubmitting } = useLogIn();

  return (
    <>
      <Form {...form}>
        <form
          className="flex w-full flex-grow flex-col justify-between"
          onSubmit={form.handleSubmit(onSubmit)}
        >
          <div className="h-[250px] w-full space-y-[10px] px-2">
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel htmlFor="log-in-email">Email</FormLabel>
                  <Input {...field} autoComplete="email" id="log-in-email" />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel htmlFor="log-in-password">Password</FormLabel>
                  <PasswordInput
                    {...field}
                    autoComplete="current-password"
                    id="log-in-password"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <AuthFormFooter
            authMode={authModes.logIn}
            disabled={disabled}
            isSubmitting={isSubmitting}
            switchToSignUp={switchToSignUp}
          />
        </form>
      </Form>
    </>
  );
};

export default LogInForm;
