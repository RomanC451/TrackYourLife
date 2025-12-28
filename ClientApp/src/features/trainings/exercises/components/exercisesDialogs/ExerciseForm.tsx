import { useState } from "react";
import { UseFormReturn } from "react-hook-form";
import { v4 as uuidv4 } from "uuid";

import { Button } from "@/components/ui/button";
import ButtonWithLoading from "@/components/ui/button-with-loading";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { InputError } from "@/components/ui/input-error";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { Difficulty, ExerciseSet, ImagesApi } from "@/services/openapi/api";

import {
  exerciseFormSchema,
  ExerciseFormSchema,
} from "../../data/exercisesSchemas";
import ExerciseSetRow from "./exerciseSetRows/ExerciseSetRow";
import SetTypeDropdownMenu from "./exerciseSetRows/SetTypeDropdownMenu";
import FileDropZone from "./FileDropZone";
import { MuscleGroupSelect } from "./MuscleGroupSelect";

const imagesApi = new ImagesApi();

function ExerciseForm({
  tab,
  setTab,
  form,
  handleCustomSubmit,
  submitButtonText,
  pendingState,
}: {
  tab: string;
  setTab: (tab: string) => void;
  form: UseFormReturn<ExerciseFormSchema>;
  handleCustomSubmit: (
    event: React.FormEvent<HTMLFormElement>,
    skipValidation: boolean,
  ) => void;
  submitButtonText: string;
  pendingState: MutationPendingState;
}) {
  "use no memo";
  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = await form.trigger().then((result) => {
      if (!result) {
        const errorsKeys = Object.keys(exerciseFormSchema.shape)
          .map((key) =>
            form.getFieldState(key as keyof ExerciseFormSchema).error ===
            undefined
              ? null
              : key,
          )
          .filter((key) => key !== null);

        const tab =
          errorsKeys.length > 1 || !errorsKeys.includes("exerciseSets")
            ? "details"
            : "sets";
        setTab(tab);
        return false;
      }
      return true;
    });

    if (!result) {
      return;
    }
    form.clearErrors();

    handleCustomSubmit(event, true);
  };

  const [isUploading, setIsUploading] = useState(false);

  return (
    <Form {...form}>
      <form onSubmit={onSubmit} className="space-y-4 pt-2">
        <Tabs value={tab} onValueChange={setTab} defaultValue="details">
          <TabsList className="w-full">
            <TabsTrigger className="w-full" value="details">
              Details
            </TabsTrigger>
            <TabsTrigger className="w-full" value="sets" disabled={isUploading}>
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
                    autoComplete="off"
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            <MuscleGroupSelect />
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
            <FormField
              control={form.control}
              name="pictureUrl"
              render={({ field }) => (
                <FormItem>
                  <Label htmlFor="pictureUrl" className="text-right">
                    Image
                  </Label>
                  <FileDropZone
                    uploadFunction={(file) => {
                      return imagesApi
                        .uploadImage(file)
                        .then((res) => res.data);
                    }}
                    onSuccess={(data) => {
                      form.setValue("pictureUrl", data);
                    }}
                    onRemove={() => {
                      form.setValue("pictureUrl", "");
                    }}
                    defaultImageUrl={field.value}
                    setIsPending={setIsUploading}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </TabsContent>
          <TabsContent value="sets">
            <div className="space-y-4">
              <div className="flex flex-col gap-4">
                <Label>Sets Type</Label>
                <SetTypeDropdownMenu />
              </div>
              {form.watch("exerciseSets").map((_, index) => (
                <ExerciseSetRow
                  key={`${form.watch("name")}-${index}`}
                  index={index}
                  form={form}
                />
              ))}

              <Button
                type="button"
                variant="outline"
                className="w-full"
                onClick={() => {
                  const currentSets = form.getValues("exerciseSets") || [];
                  form.setValue("exerciseSets", [
                    ...currentSets,
                    createDefaultExerciseSet(currentSets),
                  ]);
                }}
              >
                Add Set
              </Button>
            </div>
          </TabsContent>
          <Separator className="my-4" />
          <div className="flex flex-col gap-2">
            <div className="flex items-center justify-end gap-4">
              <InputError
                isError={form.getFieldState("exerciseSets").error !== undefined}
                message={
                  form.getFieldState("exerciseSets").error?.root?.message ?? ""
                }
              />
              <ButtonWithLoading
                type="submit"
                disabled={pendingState.isPending || isUploading}
                isLoading={pendingState.isDelayedPending}
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

function createDefaultExerciseSet(currentSets: ExerciseSet[]): ExerciseSet {
  if (currentSets.length == 0) {
    return {
      id: uuidv4(),
      name: "Set 1",
      orderIndex: 0,
      count1: 0,
      unit1: "kg",
      count2: 0,
      unit2: "reps",
    };
  }
  const existingSet = currentSets[0];

  return {
    ...existingSet,
    count1: 0,
    count2: existingSet.count2 ? 0 : undefined,
  };
}
