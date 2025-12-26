import { z } from "zod";

export const logInSchema = z.object({
  email: z.string().nonempty("Email required.").email(),
  password: z.string().nonempty("Password required."),
});

export type LogInSchema = z.infer<typeof logInSchema>;

export const signUpSchema = z
  .object({
    email: z.string().nonempty("Email required.").email(),
    password: z
      .string()
      .nonempty("Password required.")
      .min(10, "Password must be at least 10 characters")
      .regex(
        /^(?=.*[A-Z])/,
        "Password must contain at least one uppercase letter.",
      )
      .regex(
        /^(?=.*[a-z])/,
        "Password must contain at least one lowercase letter.",
      )
      .regex(/^(?=.*\d)/, "Password must contain at least one digit.")
      .regex(
        /^(?=.*[@#$%^&+=!.])/,
        "Password must contain at least one special character.",
      ),
    confirmPassword: z.string().nonempty("Confirm password required."),
    firstName: z
      .string({ required_error: "First name required." })
      .nonempty("First name required."),
    lastName: z
      .string({ required_error: "Last name required." })
      .nonempty("Last name required."),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match.",
    path: ["confirmPassword"],
  });

export type SignUpSchema = z.infer<typeof signUpSchema>;
