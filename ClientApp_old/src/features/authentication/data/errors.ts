import { ObjectValues } from "~/types/defaultTypes";

export const apiErrors = {
  General: {
    UnProcessableRequest: "General.UnProcessableRequest",
    ServerError: "General.ServerError",
  },

  Email: {
    AlreadyUsed: "Email.AlreadyInUse",
    NotVerified: "Email.NotVerified",
  },

  User: {
    InvalidCredentials: "User.InvalidCredentials",
  },
} as const;

export type apiErrors = ObjectValues<typeof apiErrors>;
