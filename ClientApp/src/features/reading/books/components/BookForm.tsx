import { FormProvider, useWatch } from "react-hook-form";
import { useEffect } from "react";
import { toast } from "sonner";

import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import useCustomForm from "@/components/forms/useCustomForm";
import { getDateOnly } from "@/lib/date";
import type { BookDto } from "@/services/openapi";
import { BookStatus } from "@/services/openapi";

import {
  bookFormSchema,
  type BookFormValues,
} from "../schemas/bookSchema";
import {
  bookStatusOptions,
  normalizeBookStatus,
  showsFinishedFields,
  showsStartingDate,
} from "../schemas/bookStatusUtils";

type BookFormProps = {
  defaultValues?: Partial<BookFormValues>;
  onSubmit: (values: BookFormValues) => Promise<void>;
  submitLabel: string;
  isSubmitting?: boolean;
};

function toDateInputValue(value?: string | null) {
  if (!value) return undefined;
  return value.slice(0, 10);
}

export function bookDtoToFormValues(book: BookDto): BookFormValues {
  const status = normalizeBookStatus(book.status);
  const needsStartDate =
    status === BookStatus.Ongoing || status === BookStatus.Finished;

  return {
    title: book.title,
    author: book.author,
    totalPages: book.totalPages,
    currentPage: book.currentPage,
    status,
    startingDate:
      toDateInputValue(book.startingDate) ??
      (needsStartDate ? getDateOnly(new Date()) : undefined),
    finishDate:
      toDateInputValue(book.finishDate) ??
      (status === BookStatus.Finished ? getDateOnly(new Date()) : undefined),
    rating: book.rating,
  };
}

const emptyDefaults: BookFormValues = {
  title: "",
  author: "",
  totalPages: 1,
  currentPage: 0,
  status: BookStatus.NotStarted,
};

function BookForm({
  defaultValues,
  onSubmit,
  submitLabel,
  isSubmitting,
}: BookFormProps) {
  const form = useCustomForm({
    formSchema: bookFormSchema,
    defaultValues: { ...emptyDefaults, ...defaultValues },
    onSubmit,
    shouldUnregister: false,
  });

  const status = normalizeBookStatus(
    useWatch({
      control: form.form.control,
      name: "status",
      defaultValue: form.form.getValues("status"),
    }),
  );

  const showStartingDate = showsStartingDate(status);
  const showFinishedFields = showsFinishedFields(status);

  useEffect(() => {
    if (status === BookStatus.NotStarted) {
      form.form.setValue("startingDate", undefined);
      form.form.setValue("finishDate", undefined);
      form.form.setValue("rating", undefined);
      return;
    }

    if (!form.form.getValues("startingDate")) {
      form.form.setValue("startingDate", getDateOnly(new Date()), {
        shouldValidate: true,
      });
    }

    if (status === BookStatus.Ongoing) {
      form.form.setValue("finishDate", undefined);
      form.form.setValue("rating", undefined);
      return;
    }

    if (!form.form.getValues("finishDate")) {
      form.form.setValue("finishDate", getDateOnly(new Date()), {
        shouldValidate: true,
      });
    }
  }, [status, form.form]);

  const handleFormSubmit = form.form.handleSubmit(onSubmit, (errors) => {
    const messages = Object.values(errors)
      .map((error) => error?.message)
      .filter((message): message is string => Boolean(message));

    if (messages.length > 0) {
      toast.error(messages.join(" · "));
    }
  });

  return (
    <FormProvider {...form.form}>
      <form onSubmit={handleFormSubmit} className="flex flex-col gap-4">
        <FormField
          control={form.form.control}
          name="title"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Title</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.form.control}
          name="author"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Author</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.form.control}
            name="totalPages"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Total pages</FormLabel>
                <FormControl>
                  <Input type="number" min={1} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.form.control}
            name="currentPage"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Current page</FormLabel>
                <FormControl>
                  <Input type="number" min={0} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <FormField
          control={form.form.control}
          name="status"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Status</FormLabel>
              <Select
                onValueChange={field.onChange}
                value={normalizeBookStatus(field.value)}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {bookStatusOptions.map((option) => (
                    <SelectItem key={option} value={option}>
                      {option}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />
        {showStartingDate && (
          <FormField
            control={form.form.control}
            name="startingDate"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Starting date</FormLabel>
                <FormControl>
                  <Input
                    type="date"
                    value={field.value ?? ""}
                    onChange={(event) =>
                      field.onChange(event.target.value || undefined)
                    }
                    onBlur={field.onBlur}
                    name={field.name}
                    ref={field.ref}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        )}
        {showFinishedFields && (
          <>
            <FormField
              control={form.form.control}
              name="finishDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Finish date</FormLabel>
                  <FormControl>
                    <Input
                      type="date"
                      value={field.value ?? ""}
                      onChange={(event) =>
                        field.onChange(event.target.value || undefined)
                      }
                      onBlur={field.onBlur}
                      name={field.name}
                      ref={field.ref}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.form.control}
              name="rating"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Rating (1–5)</FormLabel>
                  <Select
                    onValueChange={(value) => field.onChange(Number(value))}
                    value={field.value ? String(field.value) : undefined}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a rating" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {[1, 2, 3, 4, 5].map((value) => (
                        <SelectItem key={value} value={String(value)}>
                          {value}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </>
        )}
        <div className="flex justify-end gap-2">
          <ButtonWithLoading type="submit" isLoading={isSubmitting ?? false}>
            {submitLabel}
          </ButtonWithLoading>
        </div>
      </form>
    </FormProvider>
  );
}

export default BookForm;
