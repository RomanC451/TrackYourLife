/** @type {import("prettier").Config} */
const config = {
  plugins: ["prettier-plugin-tailwindcss"],
  importOrderSeparation: true,
  importOrder: ["<THIRD_PARTY_MODULES>", "^~/(.*)$", "^[./]"],
};

export default config;
