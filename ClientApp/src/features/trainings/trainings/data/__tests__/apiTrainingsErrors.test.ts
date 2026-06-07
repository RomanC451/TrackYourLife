import { describe, expect, it } from "vitest";

import { apiTrainingsErrors } from "../apiTrainingsErrors";

describe("apiTrainingsErrors", () => {
  it("exposes validation error field names", () => {
    expect(apiTrainingsErrors.ValidationErrors.Name).toBe("Name");
  });
});
