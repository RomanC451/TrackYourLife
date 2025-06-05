import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form, FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { LoadingState } from "@/hooks/useDelayedLoading";

import { ExerciseFormSchema } from "../../data/exercisesSchemas";
import ExerciseSetRow from "./ExerciseSetRow";

function ExerciseForm({
  tab,
  setTab,
  form,
  handleCustomSubmit,
  submitButtonText,
  isPending,
}: {
  tab: string;
  setTab: (tab: string) => void;
  form: UseFormReturn<ExerciseFormSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  isPending: LoadingState;
}) {
  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = await form.trigger();

    if (!result) {
      const errors = Object.keys(form.formState.errors);

      setTab(
        errors.length > 1 || !errors.includes("exerciseSets")
          ? "details"
          : "sets",
      );
      return;
    }
    handleCustomSubmit(event);
  };

  return (
    <Form {...form}>
      <form onSubmit={onSubmit} className="space-y-4 pt-2">
        <Tabs value={tab} onValueChange={setTab} defaultValue="details">
          <TabsList className="w-full">
            <TabsTrigger className="w-full" value="details">
              Details
            </TabsTrigger>
            <TabsTrigger className="w-full" value="sets">
              Sets
            </TabsTrigger>
          </TabsList>
          <TabsContent value="details" className="space-y-2">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="name" className="text-right">
                    Name
                  </Label>
                  <Input
                    {...field}
                    id="create-exercise-name"
                    className="col-span-3"
                    placeholder="Exercise name"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="description" className="text-right">
                    Description
                  </Label>
                  <Textarea
                    {...field}
                    id="create-exercise-description"
                    className="col-span-3 min-h-[200px]"
                    placeholder="Exercise description"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="equipment"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="equipment" className="text-right">
                    Equipment
                  </Label>
                  <Input
                    {...field}
                    id="create-exercise-equipment"
                    className="col-span-3"
                    placeholder="Exercise equipment"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="pictureUrl"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="pictureUrl" className="text-right">
                    Picture URL
                  </Label>
                  <Input
                    {...field}
                    id="create-exercise-pictureUrl"
                    className="col-span-3"
                    placeholder="Exercise picture URL"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="videoUrl"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="videoUrl" className="text-right">
                    Video URL
                  </Label>
                  <Input
                    {...field}
                    id="create-exercise-videoUrl"
                    className="col-span-3"
                    placeholder="Exercise video URL"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </TabsContent>
          <TabsContent value="sets">
            <div className="space-y-4">
              {form.watch("exerciseSets").map((_, index) => (
                <ExerciseSetRow key={index} index={index} form={form} />
              ))}
              <Button
                type="button"
                variant="outline"
                className="w-full"
                onClick={() => {
                  const currentSets = form.getValues("exerciseSets") || [];
                  form.setValue("exerciseSets", [
                    ...currentSets,
                    { name: "", reps: 0, weight: 0 },
                  ]);
                }}
              >
                Add Set
              </Button>
            </div>
          </TabsContent>
          <div className="flex flex-col gap-2 pt-4">
            <div className="flex items-center justify-end gap-4">
              {tab == "sets" && form.formState.errors.exerciseSets?.root && (
                <p className="text-red-800">
                  {form.formState.errors.exerciseSets.root.message}
                </p>
              )}
              <ButtonWithLoading
                type="submit"
                disabled={!isPending.isLoaded}
                isLoading={isPending.isLoading}
                // className="min-w-[100px]"
              >
                {submitButtonText}
              </ButtonWithLoading>
            </div>
          </div>
        </Tabs>
      </form>
    </Form>
  );
}

export default ExerciseForm;
