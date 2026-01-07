import z from "zod";

import { Difficulty } from "@/services/openapi";

import { exerciseFormSchema } from "../../exercises/data/exercisesSchemas";

export const trainingFormSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, { message: "Name is required" }),
  muscleGroups: z
    .array(z.string())
    .min(1, { message: "Muscle group is required" }),
  difficulty: z.nativeEnum(Difficulty),
  description: z.string().optional(),
  duration: z.number(),
  restSeconds: z.number(),
  exercises: z
    .array(exerciseFormSchema)
    .min(1, { message: "At least one exercise is required" }),
});

export type TrainingFormSchema = z.infer<typeof trainingFormSchema>;
