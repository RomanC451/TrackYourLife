/** @type {import("prettier").Config} */
const config = {
  plugins: ["prettier-plugin-tailwindcss"],
  importOrderSeparation: true,
  // trailingComma: "none",
  // arrowParens: "avoid",
  // printWidth: 80,
  // useTabs: false,
  // tabWidth: 2,
  // semi: true,
  importOrder: ["<THIRD_PARTY_MODULES>", "^~/(.*)$", "^[./]"],
  // importOrderSortSpecifiers: true
};

export default config;
