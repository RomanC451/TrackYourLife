export const authErrors = {
  EmailNotUnique: "User.Email.AlreadyInUse",
  InvalidCredentials: "User.InvalidCredentials",
  EmailNotVerified: "User.Email.NotVerified"
} as const;

type ObjectValues<T> = T[keyof T];

export type TAuthErrors = ObjectValues<typeof authErrors>;
