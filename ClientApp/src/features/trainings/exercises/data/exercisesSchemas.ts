import { z } from "zod";



export const exerciseSetSchema = z.object({
    id: z.string().optional().default(undefined!),
    name: z.string().min(1, { message: "Name is required" }),
    reps: z.number().min(1, { message: "Reps is required" }),
    weight: z.number().min(1, { message: "Weight is required" }),
    orderIndex: z.number().min(0, { message: "Order index is required" }),
  });




export const exerciseFormSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1, { message: "Name is required" }),
    description: z.string().optional(),
    equipment: z.string().optional(),
    // image: z.instanceof(File).optional(),
    videoUrl: z.string().url().optional().or(z.literal('')),
    pictureUrl: z.string().url().optional().or(z.literal('')),
    exerciseSets: z.array(exerciseSetSchema).min(1, { message: "At least one set is required" })
  });
  
  export type ExerciseFormSchema = z.infer<typeof exerciseFormSchema>;

export const exerciseSetChangeSchema = z.object({
  setId: z.string(),
  newWeight: z.number(),
  newReps: z.number(),
});

export const exerciseSetChangeFormSchema = z.object({
  changes: z.array(exerciseSetChangeSchema).min(1, { message: "At least one set change is required" })
});

export type ExerciseSetChangeSchema = z.infer<typeof exerciseSetChangeSchema>;
export type ExerciseSetChangeFormSchema = z.infer<typeof exerciseSetChangeFormSchema>;

