export const apiAuthErrors = {
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
