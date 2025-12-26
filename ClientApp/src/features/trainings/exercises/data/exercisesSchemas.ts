import { z } from "zod";

import { Difficulty, ExerciseSetType } from "@/services/openapi";

const exerciseSetSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, { message: "Name is required" }),
  orderIndex: z.number().min(0, { message: "Order index is required" }),
  restTimeSeconds: z.number().optional(),
  type: z.nativeEnum(ExerciseSetType),
});

export const weightBasedExerciseSetSchema = exerciseSetSchema.extend({
  type: z.literal(ExerciseSetType.Weight),
  reps: z.number().min(1, { message: "Reps is required" }),
  weight: z.number().min(0.1, { message: "Weight is required" }),
});

export const timeBasedExerciseSetSchema = exerciseSetSchema.extend({
  type: z.literal(ExerciseSetType.Time),
  durationSeconds: z.number().min(1, { message: "Duration is required" }),
});

export const bodyweightExerciseSetSchema = exerciseSetSchema.extend({
  type: z.literal(ExerciseSetType.Bodyweight),
  reps: z.number().min(1, { message: "Reps is required" }),
});

export const distanceExerciseSetSchema = exerciseSetSchema.extend({
  type: z.literal(ExerciseSetType.Distance),
  distance: z.number().min(0.1, { message: "Distance is required" }),
  distanceUnit: z.string().min(1, { message: "Distance unit is required" }),
});

export type WeightBasedExerciseSetSchema = z.infer<
  typeof weightBasedExerciseSetSchema
>;
export type TimeBasedExerciseSetSchema = z.infer<
  typeof timeBasedExerciseSetSchema
>;
export type BodyweightExerciseSetSchema = z.infer<
  typeof bodyweightExerciseSetSchema
>;
export type DistanceExerciseSetSchema = z.infer<
  typeof distanceExerciseSetSchema
>;
export type ExerciseSetSchema = z.infer<
  typeof exerciseSetSchema &
    (
      | typeof weightBasedExerciseSetSchema
      | typeof timeBasedExerciseSetSchema
      | typeof bodyweightExerciseSetSchema
      | typeof distanceExerciseSetSchema
    )
>;

export const exerciseFormSchema = z.object({
  id: z.string().optional(),
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
    .array(
      weightBasedExerciseSetSchema.or(
        timeBasedExerciseSetSchema.or(
          bodyweightExerciseSetSchema.or(distanceExerciseSetSchema),
        ),
      ),
    )
    .min(1, { message: "At least one set is required" }),
  // type: z.nativeEnum(ExerciseSetType),
});

export type ExerciseFormSchema = z.infer<typeof exerciseFormSchema>;

export const exerciseSetChangesSchema = z.object({
  newSets: z.array(
    weightBasedExerciseSetSchema.or(
      timeBasedExerciseSetSchema.or(
        bodyweightExerciseSetSchema.or(distanceExerciseSetSchema),
      ),
    ),
  ),
});

export type ExerciseSetChangesSchema = z.infer<typeof exerciseSetChangesSchema>;
