import { authModesEnum } from "./enums";

export const cardAnimations = {
  [authModesEnum.singUp]: { x: -1, borderRadius: "8px 0px 0px 8px" },
  [authModesEnum.logIn]: { x: 409, borderRadius: "0px 8px 8px 0px" },
};
export const cardTransitionProps = { type: "spring", duration: 1 };
