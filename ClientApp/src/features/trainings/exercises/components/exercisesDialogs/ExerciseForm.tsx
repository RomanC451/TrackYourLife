import { useState } from "react";
import { UseFormReturn } from "react-hook-form";

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
import UploadFileInput2 from "@/components/ui/upload-file-input-2";
import { MutationPendingState } from "@/hooks/useCustomMutation";
import { Difficulty, ImagesApi } from "@/services/openapi/api";

import {
  exerciseFormSchema,
  ExerciseFormSchema,
} from "../../data/exercisesSchemas";
import ExerciseSetRow from "./ExerciseSetRow";
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
  handleCustomSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  submitButtonText: string;
  pendingState: MutationPendingState;
}) {
  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const result = await form.trigger().then((result) => {
      if (!result) {
        const errorsKeys = Object.keys(exerciseFormSchema.shape)
          .map((key) =>
            form.getFieldState(key as keyof ExerciseFormSchema).error !=
            undefined
              ? key
              : null,
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

    handleCustomSubmit(event);
  };

  const [isUploading, setIsUploading] = useState(false);

  const [imageName, setImageName] = useState("");

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
                  <UploadFileInput2
                    uploadFunction={(file) => {
                      setImageName(file.name);
                      return imagesApi
                        .uploadImage(file)
                        .then((res) => res.data);
                    }}
                    onSuccess={(data) => {
                      form.setValue("pictureUrl", data);
                    }}
                    onRemove={() => {
                      form.setValue("pictureUrl", "");
                      setImageName("");
                    }}
                    defaultImageUrl={field.value}
                    defaultName={imageName}
                    setIsPending={setIsUploading}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          </TabsContent>
          <TabsContent value="sets">
            <div className="space-y-4">
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
                    {
                      id: undefined!,
                      name: "",
                      reps: 0,
                      weight: 0,
                      orderIndex: currentSets.length,
                    },
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
              {tab == "sets" &&
                !Array.isArray(form.formState.errors.exerciseSets) && (
                  <p className="text-[0.8rem] font-medium text-destructive">
                    {form.formState.errors.exerciseSets?.root?.message}
                    {form.formState.errors.exerciseSets?.message}
                  </p>
                )}
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
