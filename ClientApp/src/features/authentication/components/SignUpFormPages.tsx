import { UseFormReturn } from "react-hook-form";

import {
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import PasswordInput from "@/components/ui/password-input";

import { SignUpSchema } from "../data/schemas";

const signUpFormPages = [
  {
    formFields: ["email", "password"],
    element: (
      form: UseFormReturn<SignUpSchema, unknown, undefined>,
      updateCarouselSlide: (pageIndex: number) => void,
    ) => (
      <>
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel htmlFor="email">Email</FormLabel>
              <Input
                {...field}
                autoComplete="email"
                id="email"
                type="email"
                onFocus={() => updateCarouselSlide(0)}
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel htmlFor="password">Password</FormLabel>
              <PasswordInput
                {...field}
                autoComplete="new-password"
                id="password"
                onFocus={() => updateCarouselSlide(0)}
              />
              <FormMessage />
            </FormItem>
          )}
        />
      </>
    ),
  },
  {
    formFields: ["confirmPassword", "firstName"],
    element: (
      form: UseFormReturn<SignUpSchema, unknown, undefined>,
      updateCarouselSlide: (pageIndex: number) => void,
    ) => (
      <>
        <FormField
          control={form.control}
          name="confirmPassword"
          render={({ field }) => (
            <FormItem>
              <FormLabel htmlFor="confirmPassword">Confirm password</FormLabel>
              <PasswordInput
                {...field}
                autoComplete="new-password"
                id="confirmPassword"
                onFocus={() => updateCarouselSlide(1)}
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="firstName"
          render={({ field }) => (
            <FormItem>
              <FormLabel htmlFor="firstName">First name</FormLabel>
              <Input
                {...field}
                autoComplete="name"
                id="firstName"
                onFocus={() => updateCarouselSlide(1)}
              />
              <FormMessage />
            </FormItem>
          )}
        />
      </>
    ),
  },
  {
    formFields: ["lastName"],
    element: (
      form: UseFormReturn<SignUpSchema, unknown, undefined>,
      updateCarouselSlide: (pageIndex: number) => void,
    ) => (
      <FormField
        control={form.control}
        name="lastName"
        render={({ field }) => (
          <FormItem>
            <FormLabel htmlFor="lastName">Last name</FormLabel>
            <Input
              {...field}
              autoComplete="family-name"
              id="lastName"
              onFocus={() => updateCarouselSlide(2)}
            />
            <FormMessage />
          </FormItem>
        )}
      />
    ),
  },
];

export default signUpFormPages;
