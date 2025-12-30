import { defineConfig } from "@julr/vite-plugin-validate-env";
import { z } from "zod";

export default defineConfig({
  validator: "zod",
  schema: {
    VITE_HOST: z.string(),
    VITE_HTTPS: z
      .string()
      .optional()
      .transform((value) => value === "true"),
    VITE_API_PATH: z.string().transform((value) => value.replace(/\/$/, "")),
    VITE_DEV_EMAIL: z.string().email().optional(),
    VITE_DEV_PASSWORD: z.string().min(8).optional(),
    VITE_HIDE_TOOLS: z.string().transform((value) => value === "true"),
  },
});
