import path from "node:path";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vitest/config";

export default defineConfig({
  plugins: [
    react({
      babel: {
        plugins: [["babel-plugin-react-compiler"]],
      },
    }),
  ],
  test: {
    include: ["src/**/*.{test,spec}.{ts,tsx}"],
    exclude: ["e2e/**", "playwright.config.ts"],
    globals: true,
    environment: "jsdom",
    setupFiles: "./vitest.setup.ts",
    reporters: process.env.CI
      ? [
          "default",
          ["junit", { outputFile: "./TestResults/vitest-junit.xml" }],
          "github-actions",
        ]
      : ["default"],
    coverage: {
      provider: "custom",
      customProviderModule: "vitest-monocart-coverage",
      reportsDirectory: "./coverage",
      // Use --pool=forks --maxWorkers=1 when generating coverage to avoid temp-file races.
      // include: ["src/**/*.{ts,tsx}"],
      exclude: [
        "src/**/*.test.{ts,tsx}",
        "src/**/__tests__/**",
        "src/**/__test__/**",
        "src/services/openapi/**",
        "src/routeTree.gen.ts",
      ],
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
  },
});
