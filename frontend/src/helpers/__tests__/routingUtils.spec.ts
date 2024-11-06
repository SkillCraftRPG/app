import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import { getWorldSlug } from "../routingUtils";

describe("routingUtils.getWorldSlug", () => {
  it.concurrent("should return the correct parsed world slug", () => {
    const slug: string = "ungar";
    expect(getWorldSlug(`/worlds/${slug}/aspects/${nanoid()}`)).toBe(slug);
  });

  it.concurrent("should return undefined when there is no world slug part", () => {
    expect(getWorldSlug()).toBeUndefined();
  });
});
