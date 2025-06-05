import { zodResolver } from "@hookform/resolvers/zod";
import { DefaultValues, Path, useForm } from "react-hook-form";
import { z } from "zod";

function useCustomForm<TSchema extends z.ZodType>({
    formSchema,
    defaultValues,
    onSubmit,
  }: {
    formSchema: TSchema;
    defaultValues: DefaultValues<z.infer<TSchema>>;
    onSubmit: (formData: z.infer<TSchema>) => void;
  }
) {
    const form = useForm<z.infer<TSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: defaultValues,
    });
    
    function handleCustomSubmit(event: React.FormEvent<HTMLFormElement>) {
        const errorFields: Path<z.infer<TSchema>>[] = Object.keys(form.formState.errors)
            .map(k => k.toLowerCase() as Path<z.infer<TSchema>>)

        if (errorFields.length > 0) {
          event.preventDefault();
          form.setFocus(errorFields[0]);
          return;
        }

        form.handleSubmit(onSubmit)(event);
    }

    return { form, handleCustomSubmit };
}

export default useCustomForm;