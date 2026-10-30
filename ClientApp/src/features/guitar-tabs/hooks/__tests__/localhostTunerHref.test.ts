import { describe, expect, it } from "vitest";

import { localhostTunerHref } from "../useTunerMicrophone";

describe("localhostTunerHref", () => {
  it("keeps the port and path on localhost", () => {
    expect(
      localhostTunerHref({
        port: "5173",
        pathname: "/guitar-tabs/tuner",
        search: "",
      }),
    ).toBe("http://127.0.0.1:5173/guitar-tabs/tuner");
  });
});
