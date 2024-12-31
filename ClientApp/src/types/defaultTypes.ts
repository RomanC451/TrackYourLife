export type TJsonObject = Record<string, unknown>;

export type ObjectValues<T> = T[keyof T];

export type PartialWithRequired<T, K extends keyof T> = Partial<T> &
  Required<Pick<T, K>>;
