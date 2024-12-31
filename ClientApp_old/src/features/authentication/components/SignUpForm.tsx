import React from "react";

import { Button } from "~/chadcn/ui/button";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "~/chadcn/ui/carousel";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "~/chadcn/ui/form";
import { Input } from "~/chadcn/ui/input";
import PasswordInput from "~/chadcn/ui/password-input";
import useSignup from "../hooks/useSignup";

import { CircularProgress } from "@mui/material";
import { authModesEnum } from "../data/enums";
import ThirdAppsAuth from "./ThirdAppsAuth";

type SignUpFormProps = {
  switchToLogIn: () => void;
  isAnimating: boolean;
};

/**
 * SignUpForm component for user registration.
 * @returns React component
 */
const SignUpForm: React.FC<SignUpFormProps> = ({
  switchToLogIn,
  isAnimating,
}) => {
  const { form, onSubmit, isSubmitting } = useSignup();
  return (
    <>
      <Form {...form}>
        <form
          className="flex w-full flex-grow flex-col justify-between"
          onSubmit={form.handleSubmit(onSubmit)}
        >
          <Carousel keyControllable={false}>
            <div className="flex h-[250px] w-full flex-col justify-between">
              <CarouselContent>
                <CarouselItem key={1} className="mb-2">
                  <div className="space-y-[10px] px-2 ">
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
                </CarouselItem>
                <CarouselItem key={2}>
                  <div className="space-y-[10px] px-2">
                    <FormField
                      control={form.control}
                      name="confirmPassword"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Confirm password</FormLabel>
                          <PasswordInput {...field} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="firstName"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>First name</FormLabel>
                          <Input {...field} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </CarouselItem>
                <CarouselItem key={3}>
                  <div className="space-y-[10px] px-2">
                    <FormField
                      control={form.control}
                      name="lastName"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Last name</FormLabel>
                          <Input {...field} />
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </CarouselItem>
              </CarouselContent>

              <div className="mb-2 flex w-full justify-end gap-2">
                <CarouselPrevious
                  type="button"
                  className="static translate-y-0"
                />
                <CarouselNext type="button" className="static translate-y-0" />
              </div>
            </div>
          </Carousel>
          <Button
            type="submit"
            disabled={isAnimating || isSubmitting}
            className="w-full"
          >
            <div className="relative ">
              {isSubmitting ? (
                <CircularProgress size={20} className="absolute -left-8" />
              ) : null}
              <span>Sign Up</span>
            </div>
          </Button>
          <ThirdAppsAuth
            disabled={isAnimating || isSubmitting}
            authMode={authModesEnum.singUp}
          />
          <Button
            type="button"
            variant="ghost"
            onClick={switchToLogIn}
            disabled={isAnimating || isSubmitting}
          >
            I already have an account.
          </Button>
        </form>
      </Form>
    </>
  );
};

export default SignUpForm;
