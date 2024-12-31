import { ObjectValues } from "@/types/defaultTypes";

export const authModes = {
  logIn: "logIn",
  singUp: "singUp",
} as const;

export type AuthMode = ObjectValues<typeof authModes>;
