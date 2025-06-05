import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import { Form, FormField, FormItem, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { LoadingState } from "@/hooks/useDelayedLoading";

import { TrainingFormSchema } from "../../data/trainingsSchemas";
import ExercisesFormList from "../exercisesFormList/ExercisesFormList";

function TrainingForm({
  tab,
  setTab,
  form,
  handleCustomSubmit,
  submitButtonText,
  isPending,
  onCancel,
}: {
  form: UseFormReturn<TrainingFormSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  isPending: LoadingState;
  tab: string;
  setTab: (tab: string) => void;
  onCancel: () => void;
}) {
  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = await form.trigger();

    if (!result) {
      const errors = form.formState.errors;
      setTab(errors.exercises ? "exercises" : "details");
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
            <TabsTrigger className="w-full" value="exercises">
              Exercises
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
                    id="create-training-name"
                    className="col-span-3"
                    placeholder="Training name"
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
                    id="create-training-description"
                    className="col-span-3"
                    placeholder="Workout description"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="duration"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="duration" className="text-right">
                    Duration (minutes)
                  </Label>
                  <Input
                    {...field}
                    value={field.value === 0 ? "" : field.value}
                    id="create-training-duration"
                    type="number"
                    onChange={(e) => {
                      const value = e.target.value;
                      field.onChange(value === "" ? 0 : Number(value));
                    }}
                    className="col-span-3"
                    placeholder="Enter duration in minutes"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex justify-end gap-2 pt-4">
              <Button variant="outline" type="button" onClick={onCancel}>
                Cancel
              </Button>
              <ButtonWithLoading
                type="submit"
                disabled={!isPending.isLoaded}
                isLoading={isPending.isLoading}
                className="min-w-[100px]"
              >
                {submitButtonText}
              </ButtonWithLoading>
            </div>
          </TabsContent>
          <TabsContent value="exercises" className="space-y-4">
            <FormField
              control={form.control}
              name="exercises"
              render={() => (
                <FormItem>
                  <ExercisesFormList
                    form={form}
                    isPending={isPending}
                    onCancel={onCancel}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </TabsContent>
        </Tabs>
      </form>
    </Form>
  );
}

export default TrainingForm;
