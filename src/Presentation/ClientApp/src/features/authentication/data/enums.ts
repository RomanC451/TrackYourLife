import { ObjectValues } from "~/types/defaultTypes";

export const authModesEnum = {
  logIn: "logIn",
  singUp: "singUp",
} as const;

export type TAuthModes = ObjectValues<typeof authModesEnum>;

export const severityEnum = {
  success: "success",
  info: "info",
  warning: "warning",
  error: "error",
} as const;

export type TSeverityEnum = ObjectValues<typeof severityEnum>;

export const authAlertEnum = {
  unknown: { message: "", severity: severityEnum.info },
  good: { message: "Log in Successfully", severity: severityEnum.success },
  successfulRegister: {
    message: "You have successfully registered. Please verify your email.",
    severity: severityEnum.success,
  },
  wrongCredentials: {
    message: "Wrong credentials! Try again.",
    severity: severityEnum.warning,
  },
  somethingWrong: {
    message: "Someting went wrong. Please retry later.",
    severity: severityEnum.error,
  },
} as const;

export type TAuthAlertEnum = { message: string; severity: TSeverityEnum };
