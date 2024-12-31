import React, { useEffect, useRef, useState } from "react";

import {
  Carousel,
  CarouselApi,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { Form } from "@/components/ui/form";
import { cn } from "@/lib/utils";

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

  const [currentCarouselIndex, setCurrentCarouselIndex] = useState(0);

  const apiRef = useRef<CarouselApi | null>(null);

  useEffect(() => {
    apiRef.current = api;
    api?.on("scroll", (api) => {
      setCurrentCarouselIndex(api.selectedScrollSnap());
    });
  }, [api]);

  const updateCarouselSlide = (pageIndex: number) => {
    apiRef.current?.reInit({ startIndex: pageIndex });
    setCurrentCarouselIndex(pageIndex);
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

              <div className="inline-flex h-8 items-center justify-center gap-2">
                {signUpFormPages.map((_, index) => (
                  <div
                    key={`carousel-${index}`}
                    className={cn(
                      "size-4 rounded-full border-2 border-foreground",
                      index === currentCarouselIndex
                        ? "bg-secondary-foreground"
                        : "",
                    )}
                  />
                ))}
              </div>
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
