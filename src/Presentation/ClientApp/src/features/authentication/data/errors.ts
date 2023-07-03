export enum registerErrors {
  "Name.Empty" = "The name is empty",
  "Email.InvalidFormat" = "The email is invalid",
  "Email.Empty" = "The email is empty"
  // "Password.Empty" = "The password is empty",
  // "Password.Length" = "The password must be at least 6 characters"
}

export function getRegisterInputAndError(errorType: string): [string, string] {
  const inputName = errorType.includes(".")
    ? errorType.split(".")[0].toLowerCase()
    : errorType;

  if (errorType in registerErrors) {
    const errorMessage =
      registerErrors[errorType as keyof typeof registerErrors];
    return [inputName, errorMessage];
  }

  return ["", ""];
}
