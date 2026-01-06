import { useCallback, useEffect, useRef } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { DefaultValues, Path, useForm } from "react-hook-form";
import { z } from "zod";

import useCustomSessionStorage from "./useCustomSessionStorage";

function useCustomForm<TSchema extends z.ZodType>({
  formSchema,
  defaultValues,
  onSubmit,
  sessionStorageKey,
  queryData,
  updateAlwaysOnInvalidation: updateAlwaysOnInvalidationKeys,
}: {
  formSchema: TSchema;
  defaultValues: DefaultValues<z.infer<TSchema>>;
  onSubmit: (formData: z.infer<TSchema>) => Promise<void>;
  sessionStorageKey?: string;
  queryData?: z.infer<TSchema>;
  updateAlwaysOnInvalidation?: (keyof z.infer<TSchema>)[];
}) {
  const { sessionData, setSessionData, isDirty } = useCustomSessionStorage<
    DefaultValues<z.infer<TSchema>> | undefined
  >(sessionStorageKey, queryData);

  const form = useForm<z.infer<TSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: sessionData ?? defaultValues,
  });

  // Track previous queryData to only sync when it actually changes
  const previousQueryDataRef = useRef(queryData);

  function handleCustomSubmit(
    event: React.FormEvent<HTMLFormElement>,
    skipValidation: boolean = false,
  ) {
    if (skipValidation) {
      form.handleSubmit(onSubmit)(event);
      return;
    }

    const errorFields: Path<z.infer<TSchema>>[] = Object.keys(
      form.formState.errors,
    ).map((k) => k.toLowerCase() as Path<z.infer<TSchema>>);

    if (errorFields.length > 0) {
      event.preventDefault();
      form.setFocus(errorFields[0]);
      return;
    }

    form.handleSubmit(onSubmit)(event);
  }

  // Sync form defaults when queryData changes (not when sessionData changes from form.watch())
  // This prevents infinite loops while still allowing form defaults to update when query data changes
  useEffect(() => {
    // Only sync if queryData actually changed (not if sessionData changed from form updates)
    if (queryData && previousQueryDataRef.current !== queryData) {
      previousQueryDataRef.current = queryData;

      // Only proceed if we have sessionData to sync from
      if (sessionData) {
        let dirtyFields;

        if (updateAlwaysOnInvalidationKeys) {
          dirtyFields = Object.fromEntries(
            Object.entries(form.formState.dirtyFields).filter(
              ([key]) =>
                !updateAlwaysOnInvalidationKeys.includes(
                  key as keyof z.infer<TSchema>,
                ),
            ),
          );
        } else {
          dirtyFields = form.formState.dirtyFields;
        }

        const newDefaultValues = Object.fromEntries(
          Object.entries(sessionData).filter(
            ([key]) => !dirtyFields[key as keyof typeof dirtyFields],
          ),
        );

        const currentValues = form.getValues();

        form.reset(sessionData, {
          keepValues: true,
        });

        form.reset(
          {
            ...currentValues,
            ...newDefaultValues,
          },
          {
            keepDefaultValues: true,
          },
        );
      }
    }
  }, [queryData, sessionData, form, updateAlwaysOnInvalidationKeys]);

  useEffect(() => {
    if (sessionStorageKey) {
      const subscription = form.watch((value) => {
        setSessionData(value);
      });

      return () => subscription.unsubscribe();
    }
  }, [form, setSessionData, sessionStorageKey]);

  const resetSessionStorage = useCallback(() => {
    if (sessionStorageKey) {
      setSessionData(undefined);
    }
  }, [sessionStorageKey, setSessionData]);

  const setDataIfNoSessionStorage = useCallback(
    (values?: z.TypeOf<TSchema> | DefaultValues<z.TypeOf<TSchema>>) => {
      if (!sessionData) {
        form.reset(values);
      }
    },
    [form, sessionData],
  );

  return {
    form: {
      ...form,
      resetSessionStorage,
      setDataIfNoSessionStorage,
      isDirty,
    },
    handleCustomSubmit,
  };
}

export default useCustomForm;
