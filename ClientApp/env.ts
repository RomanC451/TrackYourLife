import { defineConfig } from "@julr/vite-plugin-validate-env";
import { z } from "zod";

export default defineConfig({
  validator: "zod",
  schema: {
    VITE_HOST: z.string().ip(),
    VITE_API_PATH: z
      .string()
      .url()
      .transform((value) => value.replace(/\/$/, "")),
  },
});
