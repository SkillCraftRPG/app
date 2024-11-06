import { describe, it, expect } from "vitest";

import { decode } from "../jwtUtils";

describe("jwtUtils.decode", () => {
  it.concurrent("should return the correct parsed JWT payload", () => {
    const token: string =
      "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0NzM1MDlhMi1mZjI2LTQ2ZTEtOTM1Ni0zMzcwMmU1OWVlNTkifQ.r9BVyKZTrU0AuWOESyJsjDflMA-bKGpdvfqnRZ4gyDU";
    const parsed: unknown = decode(token);
    const entries: [string, any][] = Object.entries(parsed as object);
    expect(entries.length).toBe(1);
    expect(entries[0][0]).toBe("sub");
    expect(entries[0][1]).toBe("473509a2-ff26-46e1-9356-33702e59ee59");
  });
});
