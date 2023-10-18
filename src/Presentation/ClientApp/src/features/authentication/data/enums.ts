export const authModes = {
  logIn: "logIn",
  singUp: "singUp"
} as const;

type ObjectValues<T> = T[keyof T];

export type TAuthModes = ObjectValues<typeof authModes>;
