/** @type {import('ts-jest').JestConfigWithTsJest} */
export const preset = "ts-jest";
export const testEnvironment = "jsdom";
export const moduleNameMapper = {
  "^~/(.*)$": "<rootDir>/src",
  "\\.(css|less)$": "<rootDir>/test/jest/__mocks__/styleMock.js"
};
export const transform = {
  "^.+\\.ts?$": "ts-jest"
};
export const transformIgnorePatterns = ["<rootDir>/node_modules/"];

export const config = { resetMocks: false };
