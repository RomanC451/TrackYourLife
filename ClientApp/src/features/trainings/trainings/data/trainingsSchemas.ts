import z from "zod";

import { Difficulty } from "@/services/openapi";

import {
  exerciseFormSchema,
  // exerciseSetSchema,
} from "../../exercises/data/exercisesSchemas";

export const trainingFormSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, { message: "Name is required" }),
  muscleGroups: z.array(z.string()),
  difficulty: z.nativeEnum(Difficulty),
  description: z.string().optional(),
  duration: z.number(),
  restSeconds: z.number(),
  exercises: z
    .array(
      exerciseFormSchema,
      // z.object({
      //   id: z.string().optional(),
      //   name: z.string().min(1, { message: "Name is required" }),
      //   description: z.string().optional(),
      //   muscleGroups: z.array(z.string()).optional(),
      //   difficulty: z.string().optional(),
      //   equipment: z.string().optional(),
      //   image: z.instanceof(File).optional(),
      //   videoUrl: z.string().url().optional().or(z.literal("")).or(z.null()),
      //   exerciseSets: z
      //     .array(exerciseSetSchema)
      //     .min(1, { message: "At least one set is required" }),
      // }),
    )
    .min(1, { message: "At least one exercise is required" }),
});

export type TrainingFormSchema = z.infer<typeof trainingFormSchema>;
