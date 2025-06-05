import { z } from "zod";




export const exerciseSetSchema = z.object({
    name: z.string().min(1, { message: "Name is required" }),
    reps: z.number().min(1, { message: "Reps is required" }),
    weight: z.number().min(1, { message: "Weight is required" }),
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

