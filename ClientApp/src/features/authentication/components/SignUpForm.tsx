import React, { useRef, useState } from "react";

import {
  Carousel,
  CarouselApi,
  CarouselContent,
  CarouselDots,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { Form } from "@/components/ui/form";

import { authModes } from "../data/enums";
import useSignUp from "../hooks/useSignUp";
import AuthFormFooter from "./AuthFormFooter";
import signUpFormPages from "./SignUpFormPages";

type SignUpFormProps = {
  switchToLogIn: () => void;
  disabled: boolean;
};

const SignUpForm: React.FC<SignUpFormProps> = ({ switchToLogIn, disabled }) => {
  const { form, onSubmit, isSubmitting } = useSignUp(focusErrorFields);

  const [api, setApi] = useState<CarouselApi>();

  const apiRef = useRef<CarouselApi | null>(null);

  const updateCarouselSlide = (pageIndex: number) => {
    apiRef.current?.reInit({ startIndex: pageIndex });
  };

  function focusErrorFields() {
    const errorFields = Object.keys(form.formState.errors);

    if (!api || errorFields.length === 0) return;

    const carouselItemIndex = signUpFormPages.findIndex((page) =>
      page.formFields.some((field) => errorFields.includes(field)),
    );

    if (carouselItemIndex === -1) return;

    api.scrollTo(carouselItemIndex);
  }

  function localOnSubmit(event: React.FormEvent<HTMLFormElement>) {
    const errorFields = Object.keys(form.formState.errors);
    if (errorFields.length > 0) {
      event.preventDefault();
      focusErrorFields();
      return;
    }
    form.handleSubmit(onSubmit)(event);
  }

  return (
    <>
      <Form {...form}>
        <form
          className="flex flex-grow flex-col justify-between"
          onSubmit={localOnSubmit}
        >
          <Carousel setApi={setApi} keyControllable={false}>
            <div className="relative flex h-[250px] w-full flex-col justify-between">
              <CarouselContent>
                {signUpFormPages.map((page, index) => (
                  <CarouselItem
                    key={index}
                    className="space-y-[10px] pb-1 pl-6 pr-2"
                  >
                    {page.element(form, updateCarouselSlide)}
                  </CarouselItem>
                ))}
              </CarouselContent>

              <CarouselDots />
              <div className="absolute bottom-0 flex w-full justify-end gap-2">
                <CarouselPrevious
                  type="button"
                  className="static translate-y-0"
                />
                <CarouselNext type="button" className="static translate-y-0" />
              </div>
            </div>
          </Carousel>
          <AuthFormFooter
            authMode={authModes.singUp}
            disabled={disabled}
            isSubmitting={isSubmitting}
            switchToSignUp={switchToLogIn}
          />
        </form>
      </Form>
    </>
  );
};

export default SignUpForm;
