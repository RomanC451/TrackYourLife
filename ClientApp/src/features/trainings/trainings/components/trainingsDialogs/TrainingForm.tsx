import { ArrowRight } from "lucide-react";
import { UseFormReturn } from "react-hook-form";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { MuscleGroupSelect } from "@/features/trainings/exercises/components/exercisesDialogs/MuscleGroupSelect";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { Difficulty } from "@/services/openapi";

import { TrainingFormSchema } from "../../data/trainingsSchemas";
import ExercisesFormList from "../exercisesFormList/ExercisesFormList";

function TrainingForm({
  tab,
  setTab,
  form,
  handleCustomSubmit,
  submitButtonText,
  pendingState,
  onCancel,
}: {
  form: UseFormReturn<TrainingFormSchema>;
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  pendingState: MutationPendingState;
  tab: "details" | "exercises";
  setTab: (tab: "details" | "exercises") => void;
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
        <Tabs
          value={tab}
          onValueChange={(value) => setTab(value as "details" | "exercises")}
          defaultValue="details"
        >
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
                    autoComplete="off"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <MuscleGroupSelect />
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
              name="difficulty"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="difficulty" className="text-right">
                    Difficulty
                  </Label>
                  <Select
                    onValueChange={(value) => {
                      field.onChange(value as Difficulty);
                    }}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a difficulty" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="Easy">Easy</SelectItem>
                      <SelectItem value="Medium">Medium</SelectItem>
                      <SelectItem value="Hard">Hard</SelectItem>
                    </SelectContent>
                  </Select>
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
                    Exercise duration (minutes)
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
                    placeholder="Estimated exercise duration in minutes"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex items-center justify-end gap-2">
              <Button variant="outline" type="button" onClick={onCancel}>
                Cancel
              </Button>
              <Button
                variant="default"
                size="icon"
                type="button"
                onClick={() => {
                  setTab("exercises");
                }}
              >
                <ArrowRight />
              </Button>
            </div>
          </TabsContent>

          <TabsContent value="exercises" className="space-y-4">
            <FormField
              control={form.control}
              name="exercises"
              render={() => (
                <FormItem>
                  <ExercisesFormList
                    pendingState={pendingState}
                    onCancel={onCancel}
                    submitButtonText={submitButtonText}
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
