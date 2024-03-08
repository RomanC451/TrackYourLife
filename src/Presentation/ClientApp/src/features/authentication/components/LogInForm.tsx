import React from "react";

import { Button } from "~/chadcn/ui/button";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "~/chadcn/ui/form";
import { Input } from "~/chadcn/ui/input";
import PasswordInput from "~/chadcn/ui/password-input";

import { CircularProgress } from "@mui/material";
import { authModesEnum } from "../data/enums";
import useLogIn from "../hooks/useLogIn";
import ThirdAppsAuth from "./ThirdAppsAuth";

type LogInFormProps = {
  switchToSignUp: () => void;
  isAnimating: boolean;
};

const LogInForm: React.FC<LogInFormProps> = ({
  switchToSignUp,
  isAnimating,
}): JSX.Element => {
  const { form, onSubmit, isSubmitting: isSubmiting } = useLogIn();
  console.log(isSubmiting);

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
                  <FormLabel>Email</FormLabel>
                  <Input {...field} />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Password</FormLabel>
                  <PasswordInput {...field} />
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <Button
            type="submit"
            disabled={isAnimating || isSubmiting}
            className=""
          >
            <div className="relative ">
              {isSubmiting ? (
                <CircularProgress size={20} className="absolute -left-8" />
              ) : null}
              <span>Log In</span>
            </div>
          </Button>
          <ThirdAppsAuth
            disabled={isAnimating || isSubmiting}
            authMode={authModesEnum.logIn}
          />
          <Button
            type="button"
            variant="ghost"
            onClick={switchToSignUp}
            disabled={isAnimating || isSubmiting}
          >
            I don't have an account.
          </Button>
        </form>
      </Form>
    </>
  );
};

export default LogInForm;
