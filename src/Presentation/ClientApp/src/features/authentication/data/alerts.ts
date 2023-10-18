export enum severityEnum {
  success = "success",
  info = "info",
  warning = "warning",
  error = "error"
}

export const authAlerts = {
  unknown: { message: "", severity: severityEnum.info },
  good: { message: "Log in Successfully", severity: severityEnum.success },
  successfulRegister: {
    message: "You have successfully registered. Please verify your email.",
    severity: severityEnum.success
  },
  wrongCredentials: {
    message: "Wrong credentials! Try again.",
    severity: severityEnum.warning
  },
  somethingWrong: {
    message: "Someting went wrong. Please retry later.",
    severity: severityEnum.error
  }
};

export type authAlertType = { message: string; severity: severityEnum };
