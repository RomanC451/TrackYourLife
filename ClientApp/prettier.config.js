/** @type {import("prettier").Config} */
const config = {
  plugins: [
    "@ianvs/prettier-plugin-sort-imports",
    "prettier-plugin-tailwindcss",
  ],
  importOrderSeparation: true,
  importOrder: [
    "^react$",

    "<THIRD_PARTY_MODULES>",
    "",
    "^(@)(/.*)$",
    "",
    "^[.]",
  ],
  tailwindStylesheet: "./src/index.css",
  tailwindConfig: "./tailwind.config.js",
  tailwindFunctions: ["cn"],
};

export default config;
