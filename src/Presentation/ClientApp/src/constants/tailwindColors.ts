import { ObjectValues } from "~/types/defaultTypes";

export const colors = {
  "sidebar-gray": "#D9D9D9",
  violet: "#6F4BDA",
  "dark-violet": "#382C60",
  gray: "#434343",
  green: "#84BE80",
  yellow: "#D5B878",
  black: "#000000",
  turquoise: "#80ADB8",
  red: "#DA4B4B",
  "border-gray": "#27272A"
} as const;

export type TColors = ObjectValues<typeof colors>;

export const bgColors = {
  "login-bg": "rgb(24,25,25)",
  "main-bg": "#FAFBFB",
  "gray-bg": "#808080",
  "light-gray": "#F7F7F7",
  "half-transparent": "rgba(0, 0, 0, 0.5)",
  "main-dark-bg": "	#0c0a09",
  "second-gray-bg": "	#1C1917"
} as const;

export type TBgColors = ObjectValues<typeof bgColors>;
