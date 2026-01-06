import { z } from "zod";

import { Difficulty } from "@/services/openapi";
import { ObjectValues } from "@/types/defaultTypes";

const exerciseSetFormSchema = z.object({
  id: z.string(),
  name: z.string().min(1, { message: "Name is required" }),
  orderIndex: z.number().min(0, { message: "Order index is required" }),
  restTimeSeconds: z.number().optional(),
  count1: z.number().min(1, { message: "Required" }),
  unit1: z.string(),
  count2: z.number().min(1, { message: "Required" }).optional(),
  unit2: z.string().optional(),
});

export type ExerciseSetFormSchema = z.infer<typeof exerciseSetFormSchema>;

export const exerciseFormSchema = z.object({
  id: z.string(),
  name: z.string().min(1, { message: "Name is required" }),
  muscleGroups: z
    .array(z.string())
    .min(1, { message: "Muscle group is required" }),
  difficulty: z.nativeEnum(Difficulty),
  description: z.string().optional(),
  equipment: z.string().optional(),
  videoUrl: z.string().url().optional().or(z.literal("")),
  pictureUrl: z.string().optional().or(z.literal("")),
  exerciseSets: z
    .array(exerciseSetFormSchema)
    .min(1, { message: "At least one set is required" }),
});

export type ExerciseFormSchema = z.infer<typeof exerciseFormSchema>;

export const exerciseSetChangesSchema = z.object({
  newSets: z.array(exerciseSetFormSchema),
});

export type ExerciseSetChangesSchema = z.infer<typeof exerciseSetChangesSchema>;

export const exerciseSetType = {
  Weight: "Weight",
  Time: "Time",
  Bodyweight: "Bodyweight",
  Distance: "Distance",
} as const;

export type ExerciseSetType = ObjectValues<typeof exerciseSetType>;

export const exerciseSetTypes = [
  { label: "Weight", value: exerciseSetType.Weight },
  { label: "Time", value: exerciseSetType.Time },
  { label: "Bodyweight", value: exerciseSetType.Bodyweight },
  { label: "Distance", value: exerciseSetType.Distance },
];
