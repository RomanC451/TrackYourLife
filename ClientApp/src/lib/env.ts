type ImportMetaEnvAugmented =
  import("@julr/vite-plugin-validate-env").ImportMetaEnvAugmented<
    typeof import("../../env").default
  >;

export const env: ImportMetaEnvAugmented = import.meta.env;
