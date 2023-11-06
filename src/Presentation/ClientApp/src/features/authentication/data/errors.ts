import { ObjectValues } from "~/types/defaultTypes";

export const authErrors = {
  EmailNotUnique: "User.Email.AlreadyInUse",
  InvalidCredentials: "User.InvalidCredentials",
  EmailNotVerified: "User.Email.NotVerified"
} as const;

export type TAuthErrors = ObjectValues<typeof authErrors>;
